using KURS_ASP.NET.Data;
using KURS_ASP.NET.Models;
using KURS_ASP.NET.Services.Interfaces;
using KURS_ASP.NET.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KURS_ASP.NET.Services
{
    public class ReferenceDataService : IReferenceDataService
    {
        private readonly ApplicationDbContext _context;

        public ReferenceDataService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HeroesPageViewModel> GetHeroesAsync(string? searchTerm = null, string? attribute = null, string? attackType = null)
        {
            var query = _context.Heroes.AsQueryable();
            var normalizedSearch = searchTerm?.Trim();
            var normalizedAttribute = attribute?.Trim();
            var normalizedAttackType = attackType?.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedSearch))
            {
                query = query.Where(h =>
                    (h.HeroName ?? string.Empty).Contains(normalizedSearch) ||
                    (h.PrimaryAttribute ?? string.Empty).Contains(normalizedSearch) ||
                    (h.AttackType ?? string.Empty).Contains(normalizedSearch));
            }

            if (!string.IsNullOrWhiteSpace(normalizedAttribute))
            {
                query = query.Where(h => h.PrimaryAttribute == normalizedAttribute);
            }

            if (!string.IsNullOrWhiteSpace(normalizedAttackType))
            {
                query = query.Where(h => h.AttackType == normalizedAttackType);
            }

            var heroes = await query
                .OrderBy(h => h.HeroName)
                .ToListAsync();

            var attributes = await _context.Heroes
                .Where(h => !string.IsNullOrWhiteSpace(h.PrimaryAttribute))
                .Select(h => h.PrimaryAttribute!)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            var attackTypes = await _context.Heroes
                .Where(h => !string.IsNullOrWhiteSpace(h.AttackType))
                .Select(h => h.AttackType!)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            return new HeroesPageViewModel
            {
                Heroes = heroes,
                Attributes = attributes,
                AttackTypes = attackTypes,
                SearchTerm = normalizedSearch ?? string.Empty,
                Attribute = normalizedAttribute ?? string.Empty,
                AttackType = normalizedAttackType ?? string.Empty,
                TotalCount = heroes.Count
            };
        }

        public async Task<ItemsPageViewModel> GetItemsAsync(string? searchTerm = null, string? category = null, bool neutralOnly = false)
        {
            var query = _context.Items.AsQueryable();
            var normalizedSearch = searchTerm?.Trim();
            var normalizedCategory = category?.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedSearch))
            {
                query = query.Where(i =>
                    (i.ItemName ?? string.Empty).Contains(normalizedSearch) ||
                    (i.Category ?? string.Empty).Contains(normalizedSearch) ||
                    (i.Subcategory ?? string.Empty).Contains(normalizedSearch) ||
                    (i.Description ?? string.Empty).Contains(normalizedSearch));
            }

            if (!string.IsNullOrWhiteSpace(normalizedCategory))
            {
                query = query.Where(i => i.Category == normalizedCategory);
            }

            if (neutralOnly)
            {
                query = query.Where(i => i.IsNeutral);
            }

            var items = await query
                .OrderBy(i => i.Category)
                .ThenBy(i => i.ItemName)
                .ToListAsync();

            var categories = await _context.Items
                .Where(i => !string.IsNullOrWhiteSpace(i.Category))
                .Select(i => i.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return new ItemsPageViewModel
            {
                Items = items,
                Categories = categories,
                SearchTerm = normalizedSearch ?? string.Empty,
                Category = normalizedCategory ?? string.Empty,
                NeutralOnly = neutralOnly,
                TotalCount = items.Count
            };
        }

        public async Task<HeroDetailsViewModel?> GetHeroDetailsAsync(int heroId)
        {
            var hero = await _context.Heroes.FirstOrDefaultAsync(h => h.HeroID == heroId);
            if (hero == null)
            {
                return null;
            }

            var similarHeroes = await _context.Heroes
                .Where(h => h.HeroID != heroId &&
                            h.PrimaryAttribute == hero.PrimaryAttribute &&
                            h.AttackType == hero.AttackType)
                .OrderBy(h => h.HeroName)
                .Take(4)
                .ToListAsync();

            var suggestedItems = await _context.Items
                .Where(i => !i.IsRemoved &&
                            ((hero.AttackType == "Melee" && (i.Category == "Defense" || i.Category == "Utility")) ||
                             (hero.AttackType == "Ranged" && (i.Category == "Magic" || i.Category == "Utility")) ||
                             (hero.PrimaryAttribute == "Agility" && i.Category == "Boots") ||
                             (hero.PrimaryAttribute == "Strength" && i.Category == "Defense")))
                .OrderBy(i => i.ItemName)
                .Take(4)
                .ToListAsync();

            return new HeroDetailsViewModel
            {
                Hero = hero,
                SimilarHeroes = similarHeroes,
                SuggestedItems = suggestedItems,
                Summary = BuildHeroSummary(hero),
                Playstyle = BuildHeroPlaystyle(hero)
            };
        }

        public async Task<ItemDetailsViewModel?> GetItemDetailsAsync(int itemId)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemID == itemId);
            if (item == null)
            {
                return null;
            }

            var relatedItems = await _context.Items
                .Where(i => i.ItemID != itemId && i.Category == item.Category)
                .OrderBy(i => i.Cost)
                .ThenBy(i => i.ItemName)
                .Take(4)
                .ToListAsync();

            var suggestedHeroes = await _context.Heroes
                .Where(h =>
                    (item.Category == "Defense" && h.PrimaryAttribute == "Strength") ||
                    (item.Category == "Magic" && h.PrimaryAttribute == "Intelligence") ||
                    (item.Category == "Boots" && h.AttackType == "Melee") ||
                    (item.IsNeutral && h.AttackType == "Ranged"))
                .OrderBy(h => h.HeroName)
                .Take(4)
                .ToListAsync();

            return new ItemDetailsViewModel
            {
                Item = item,
                RelatedItems = relatedItems,
                SuggestedHeroes = suggestedHeroes,
                Summary = BuildItemSummary(item),
                TimingAdvice = BuildTimingAdvice(item)
            };
        }

        public async Task<ComparePageViewModel> GetItemComparisonAsync(int? leftId, int? rightId)
        {
            var options = await _context.Items
                .OrderBy(i => i.ItemName)
                .Select(i => new CompareEntityOptionViewModel
                {
                    Id = i.ItemID,
                    Label = i.ItemName ?? $"Item #{i.ItemID}"
                })
                .ToListAsync();

            if (!leftId.HasValue || !rightId.HasValue)
            {
                return new ComparePageViewModel
                {
                    EntityType = "items",
                    EntityTypeLabel = "предметов",
                    LeftId = leftId,
                    RightId = rightId,
                    Options = options,
                    Summary = "Выберите два предмета, чтобы сравнить стоимость, тип и назначение."
                };
            }

            var items = await _context.Items
                .Where(i => i.ItemID == leftId || i.ItemID == rightId)
                .ToListAsync();

            if (items.Count != 2)
            {
                return new ComparePageViewModel
                {
                    EntityType = "items",
                    EntityTypeLabel = "предметов",
                    LeftId = leftId,
                    RightId = rightId,
                    Options = options,
                    Summary = "Не удалось загрузить оба предмета для сравнения."
                };
            }

            var left = items.First(i => i.ItemID == leftId);
            var right = items.First(i => i.ItemID == rightId);

            return new ComparePageViewModel
            {
                EntityType = "items",
                EntityTypeLabel = "предметов",
                LeftId = leftId,
                RightId = rightId,
                LeftTitle = left.ItemName ?? $"Item #{left.ItemID}",
                RightTitle = right.ItemName ?? $"Item #{right.ItemID}",
                Options = options,
                Metrics = new List<CompareMetricViewModel>
                {
                    CreateMetric("Категория", left.Category, right.Category),
                    CreateMetric("Подкатегория", left.Subcategory, right.Subcategory),
                    CreateMetric("Стоимость", (left.Cost ?? 0).ToString(), (right.Cost ?? 0).ToString()),
                    CreateMetric("Тип", left.IsNeutral ? "Нейтральный" : "Обычный", right.IsNeutral ? "Нейтральный" : "Обычный"),
                    CreateMetric("Удален из игры", left.IsRemoved ? "Да" : "Нет", right.IsRemoved ? "Да" : "Нет")
                },
                Summary = "Сравнение показывает разницу между слотами по стоимости и игровой роли."
            };
        }

        private static CompareMetricViewModel CreateMetric(string label, string? leftValue, string? rightValue)
        {
            return new CompareMetricViewModel
            {
                Label = label,
                LeftValue = string.IsNullOrWhiteSpace(leftValue) ? "-" : leftValue,
                RightValue = string.IsNullOrWhiteSpace(rightValue) ? "-" : rightValue
            };
        }

        private static string BuildHeroSummary(Hero hero)
        {
            return $"{hero.HeroName} относится к архетипу {(hero.PrimaryAttribute ?? "смешанных")} героев и использует {(hero.AttackType ?? "универсальный")} тип атаки.";
        }

        private static string BuildHeroPlaystyle(Hero hero)
        {
            return hero.PrimaryAttribute switch
            {
                "Strength" => "Хорошо чувствует себя в затяжных драках и часто раскрывается как фронтлайн-инициатор.",
                "Agility" => "Ориентирован на фарм, темп и усиление урона по мере роста предметов.",
                "Intelligence" => "Силен за счет контроля, магического урона и грамотного позиционирования.",
                _ => "Герой требует гибкой сборки и подстраивается под конкретный драфт."
            };
        }

        private static string BuildItemSummary(Item item)
        {
            return $"{item.ItemName} относится к категории {(item.Category ?? "Utility")} и чаще всего используется как {(item.IsNeutral ? "нейтральный слот" : "магазинный предмет")}.";
        }

        private static string BuildTimingAdvice(Item item)
        {
            if (item.IsNeutral)
            {
                return $"Предмет обычно рассматривают после появления нейтральных дропов уровня {(item.NeutralTier?.ToString() ?? "-")}.";
            }

            if ((item.Cost ?? 0) >= 4000)
            {
                return "Это предмет среднего или позднего тайминга, который заметно усиливает core-персонажа.";
            }

            if ((item.Cost ?? 0) <= 1000)
            {
                return "Предмет подходит для раннего этапа и помогает стабилизировать лайн или мобильность.";
            }

            return "Предмет чаще всего закрывает переходный этап между лайнингом и первыми ключевыми драками.";
        }
    }
}
