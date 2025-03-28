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

    if (!tabs.length) return; // Exit if no tabs on this page

    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            tabs.forEach(t => t.classList.remove('active'));
            tab.classList.add('active');

            const selected = tab.dataset.category;

            items.forEach(item => {
                const itemCategory = item.dataset.category;
                const title = item.querySelector('.category-title-wrapper');

                if (selected === 'All' || itemCategory === selected) {
                    item.style.display = 'block';
                    if (selected === 'All' && title) {
                        title.style.display = 'block';
                    } else if (title) {
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

    if (!searchInput || !resultsContainer) return;

    let debounceTimeout;

    searchInput.addEventListener('input', () => {
        clearTimeout(debounceTimeout);

        debounceTimeout = setTimeout(() => {
            const term = searchInput.value;

            fetch(`/Home/Search?term=${encodeURIComponent(term)}`)
                .then(response => response.text())
                .then(html => {
                    resultsContainer.innerHTML = html;
                })
                .catch(err => console.error('Search error:', err));
        }, 300); // debounce for 300ms
    });
});
