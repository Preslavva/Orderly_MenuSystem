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
        }, 300);
    });
});


function pollOrderStatus(orderId) {
    fetch(`/Kitchen/GetOrderStatus?orderId=${orderId}`)
        .then(response => response.text())
        .then(status => updateStatusHighlight(status.trim()))
        .catch(err => console.error("Error fetching status", err));
}

function updateStatusHighlight(currentStatus) {
    console.log("Updating Status Highlight for:", currentStatus);

    const container = document.getElementById('orderStatusButtons');
    if (!container) return;

    const buttons = container.querySelectorAll('button');
    buttons.forEach(btn => {
        const status = btn.getAttribute('data-status');
        const isSelected = status === currentStatus;

        btn.classList.remove('selected-new', 'selected-processing', 'selected-completed');

        if (isSelected) {
            switch (status) {
                case 'NEW_ORDER':
                    btn.classList.add('selected-new');
                    break;
                case 'PROCESSING':
                    btn.classList.add('selected-processing');
                    break;
                case 'COMPLETED':
                    btn.classList.add('selected-completed');
                    break;
            }
        }

        const svg = btn.querySelector('svg');
        if (status === 'PROCESSING' || status === 'COMPLETED') {
            svg.removeAttribute('fill');
            svg.removeAttribute('stroke');

            const paths = svg.querySelectorAll('path');
            paths.forEach(path => {
                path.style.fill = isSelected ? '#ffffff' : '#000000';  
                path.style.stroke = isSelected ? '#ffffff' : '#000000'; 
            });

        }
    });
}

document.addEventListener('DOMContentLoaded', () => {
    const container = document.getElementById('orderStatusButtons');
    if (container) {
        const orderId = container.getAttribute('data-order-id');
        if (orderId) {
            pollOrderStatus(orderId);
            setInterval(() => pollOrderStatus(orderId), 5000);
        }
    }
});


