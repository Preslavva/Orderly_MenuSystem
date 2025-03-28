// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function scrollToParagraph() {
    document.getElementById('target-paragraph')?.scrollIntoView({ behavior: 'smooth' });
}



document.addEventListener('DOMContentLoaded', function () {
    const tabs = document.querySelectorAll('.tab-item');
    const items = document.querySelectorAll('.menu-item');
    const titles = document.querySelectorAll('.category-title-wrapper');

    if (!tabs.length) return; 

    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            tabs.forEach(t => t.classList.remove('active'));
            tab.classList.add('active');

            const selected = tab.dataset.category;

            items.forEach(item => {
                const itemCategory = item.dataset.category;
                const title = item.querySelector('.category-title-wrapper');

                if (selected === 'All') {
                    item.style.display = 'block';
                    if (title) {
                        title.style.display = 'block';
                    }
                } else if (itemCategory === selected) {
                    item.style.display = 'block';
                    if (title) {
                        title.style.display = 'none';
                    }
                } else {
                    item.style.display = 'none';
                }
            });
        });
    });
});


document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('searchInput');
    const resultsContainer = document.getElementById('searchResults');
    const tabs = document.querySelectorAll('.tab-item');
    const DEBOUNCE_DELAY = 300;
    let debounceTimeout;

    if (!searchInput || !resultsContainer || !tabs.length) return;

    const getSelectedCategory = () => {
        const activeTab = document.querySelector('.tab-item.active');
        return activeTab?.dataset.category || 'All';
    };

    const performSearch = () => {
        const term = searchInput.value.trim();
        const selectedCategory = getSelectedCategory();
        const categoryParam = selectedCategory === 'All' ? '' : selectedCategory;

        const url = `/Home/Search?term=${encodeURIComponent(term)}&category=${encodeURIComponent(categoryParam)}`;

        fetch(url)
            .then(response => response.text())
            .then(html => {
                resultsContainer.innerHTML = html;
            })
            .catch(err => console.error('Search error:', err));
    };

    searchInput.addEventListener('input', () => {
        clearTimeout(debounceTimeout);
        debounceTimeout = setTimeout(performSearch, DEBOUNCE_DELAY);
    });

    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            tabs.forEach(t => t.classList.remove('active'));
            tab.classList.add('active');

            searchInput.value = '';

            performSearch();
        });
    });
});


