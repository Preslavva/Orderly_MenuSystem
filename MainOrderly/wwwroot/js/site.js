
    document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('searchInput');
    const resultsContainer = document.getElementById('searchResults');
    const category = new URLSearchParams(window.location.search).get('category') || '';

    if (searchInput && resultsContainer) {
        searchInput.addEventListener('input', function () {
            const term = searchInput.value;

            fetch(`/Home/Index?searchTerm=${encodeURIComponent(term)}&category=${encodeURIComponent(category)}`, {
                headers: { "X-Requested-With": "XMLHttpRequest" }
            })
                .then(response => response.text())
                .then(html => {
                    resultsContainer.innerHTML = html;
                })
                .catch(err => console.error("Search error:", err));
        });
    }
});

