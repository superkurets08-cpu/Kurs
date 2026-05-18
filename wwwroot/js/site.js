const navToggle = document.querySelector("[data-nav-toggle]");

if (navToggle) {
    navToggle.addEventListener("click", () => {
        const isOpen = document.body.classList.toggle("nav-open");
        navToggle.setAttribute("aria-expanded", String(isOpen));
    });
}

const revealItems = document.querySelectorAll(".reveal");

if ("IntersectionObserver" in window && revealItems.length > 0) {
    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                entry.target.classList.add("is-visible");
                observer.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.01,
        rootMargin: "0px 0px -8% 0px"
    });

    revealItems.forEach((item) => observer.observe(item));
} else {
    revealItems.forEach((item) => item.classList.add("is-visible"));
}

const searchForms = document.querySelectorAll(".header-search");

searchForms.forEach((form) => {
    const input = form.querySelector("input[name='query']");
    const box = form.querySelector("[data-search-suggestions]");

    if (!input || !box) {
        return;
    }

    let abortController = null;

    input.addEventListener("input", async () => {
        const value = input.value.trim();
        box.innerHTML = "";

        if (value.length < 2) {
            return;
        }

        if (abortController) {
            abortController.abort();
        }

        abortController = new AbortController();

        try {
            const response = await fetch(`/Home/SearchSuggestions?query=${encodeURIComponent(value)}`, {
                signal: abortController.signal
            });

            if (!response.ok) {
                return;
            }

            const suggestions = await response.json();
            if (!Array.isArray(suggestions) || suggestions.length === 0) {
                return;
            }

            box.innerHTML = suggestions.map((item) =>
                `<a class="search-suggestion" href="${item.url}"><strong>${item.label}</strong><span>${item.category}</span></a>`
            ).join("");
        } catch (error) {
            if (error.name !== "AbortError") {
                box.innerHTML = "";
            }
        }
    });

    input.addEventListener("blur", () => {
        setTimeout(() => {
            box.innerHTML = "";
        }, 150);
    });
});
