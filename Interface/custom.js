document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('main-content');
    const toggleButton = document.getElementById('toggle-sidebar');

    // Check if sidebar state is stored in localStorage
    const isCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';

    // Set initial state
    if (isCollapsed) {
        sidebar.classList.add('collapsed');
        mainContent.classList.add('expanded');
        toggleButton.innerHTML = '<i class="fas fa-bars"></i>';
    }

    // Toggle sidebar
    toggleButton.addEventListener('click', function () {
        sidebar.classList.toggle('collapsed');
        mainContent.classList.toggle('expanded');

        // Store state in localStorage
        const isNowCollapsed = sidebar.classList.contains('collapsed');
        localStorage.setItem('sidebarCollapsed', isNowCollapsed);

        // Update icon
        if (isNowCollapsed) {
            toggleButton.innerHTML = '<i class="fas fa-bars"></i>';
        } else {
            toggleButton.innerHTML = '<i class="fas fa-times"></i>';
        }
    });

    // Make sidebar items clickable
    const sidebarItems = document.querySelectorAll('.sidebar-nav a');
    sidebarItems.forEach(item => {
        item.addEventListener('click', function(e) {
            // Remove active class from all items
            sidebarItems.forEach(i => i.classList.remove('active'));

            // Add active class to clicked item
            this.classList.add('active');

            // If sidebar is collapsed on mobile, close it after selection
            if (window.innerWidth < 992) {
                sidebar.classList.add('collapsed');
                mainContent.classList.add('expanded');
                toggleButton.innerHTML = '<i class="fas fa-bars"></i>';
                localStorage.setItem('sidebarCollapsed', 'true');
            }
        });
    });
});