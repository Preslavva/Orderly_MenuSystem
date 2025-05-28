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

document.addEventListener('DOMContentLoaded', function () {
    const countdownDiv = document.getElementById("countdown");
    if (!countdownDiv) return;

    const orderId = countdownDiv.getAttribute("data-order-id");
    if (!orderId) return;

    function updateCountdown() {
        fetch(`/Cart/Timer?orderId=${orderId}`)
            .then(response => response.text())
            .then(time => {
                if (time === "Time's up") {
                    countdownDiv.textContent = "Time's up";
                } else {
                    countdownDiv.textContent = time;
                }
            })
            .catch(err => console.error("Error fetching timer:", err));
    }

    updateCountdown();
    setInterval(updateCountdown, 1000);
});


document.addEventListener('DOMContentLoaded', function () {
    // Poll order status for buttons and badges
    const pollOrderStatusForElements = () => {
        document.querySelectorAll('.badge[data-order-id], #orderStatusButtons[data-order-id]').forEach(element => {
            const orderId = element.getAttribute('data-order-id');
            if (orderId) {
                pollOrderStatus(orderId);
                setInterval(() => pollOrderStatus(orderId), 5000);
            }
        });
    };

    pollOrderStatusForElements();

    // Scroll active tab into view
    const activeTabLink = document.querySelector(".tab-list .tab-item.active a");
    if (activeTabLink) {
        activeTabLink.scrollIntoView({ behavior: 'smooth', inline: 'center', block: 'nearest' });
    }

    // Update status highlight
    updateStatusHighlight("@Model.Status");
});

function pollOrderStatus(orderId) {
    fetch(`/Kitchen/GetOrderStatus?orderId=${orderId}`)
        .then(response => response.text())
        .then(status => {
            const trimmedStatus = status.trim();
            console.log(`Polled status for order ${orderId}:`, trimmedStatus);
            updateBadgeColor(orderId, trimmedStatus);
            updateStatusHighlight(trimmedStatus);
        })
        .catch(err => console.error("Error fetching status", err));
}

function updateBadgeColor(orderId, status) {
    const badge = document.querySelector(`.badge[data-order-id="${orderId}"]`);
    if (!badge) return;

    badge.style.backgroundColor = '';
    badge.style.color = '';

    switch (status) {
        case 'NEW_ORDER':
            badge.innerText = 'New Order';
            badge.style.backgroundColor = '#ffc107';
            badge.style.color = '#000000';
            break;
        case 'PROCESSING':
            badge.innerText = 'Processing';
            badge.style.backgroundColor = '#0d6efd';
            badge.style.color = '#ffffff';
            break;
        case 'COMPLETED':
            badge.innerText = 'Completed';
            badge.style.backgroundColor = '#198754';
            badge.style.color = '#ffffff';
            break;
        default:
            badge.innerText = 'Unknown';
            badge.style.backgroundColor = '#6c757d';
            badge.style.color = '#ffffff';
            break;
    }
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
        if (svg && (status === 'PROCESSING' || status === 'COMPLETED')) {
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



