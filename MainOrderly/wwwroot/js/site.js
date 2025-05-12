
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


document.addEventListener("DOMContentLoaded", function () {
    // Find the active category link
    var activeTabLink = document.querySelector(".tab-list .tab-item.active a");
    if (activeTabLink) {
        // Scroll it into view within its container, centering it horizontally
        activeTabLink.scrollIntoView({ behavior: 'smooth', inline: 'center', block: 'nearest' });
    }
});
