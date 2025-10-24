window.getBootstrapTheme = () => {
    const element = document.querySelector('[data-bs-theme]');
    return element ? element.getAttribute('data-bs-theme') : 'light';
}

window.setBootstrapTheme = (theme) => {
    document.querySelector('[data-bs-theme]').setAttribute('data-bs-theme', theme);
}

window.toggleBootstrapTheme = () => {
    const currentTheme = window.getBootstrapTheme();
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    window.setBootstrapTheme(newTheme);
    return newTheme;
}

// Set theme function
window.setTheme = (theme) => {
    // We'll find the element that has the attribute data-cf-theme
    // and update it to the new theme name.
    document.querySelector('[data-cf-theme]').setAttribute('data-cf-theme', theme);
}






// Init Bootstrap5 Tooltips
function initializeTooltips() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Init Bootstrap5 Carousel
window.initCarousel = (name) => {
    // console.log("initCarousel called");
    // Selektujte carousel element koristeæi ID
    const element = document.querySelector('#' + name);
    if (element) {
        // console.log("Element found");
        const carouselInstance = new bootstrap.Carousel(element,
            {
                interval: 10000,
                ride: 'carousel'
            });
        carouselInstance.cycle();
    }
    // else {
    //    console.log("Element not found");
    //}
};

// START Summernote editor
function addSummernote(id, text) {
    text = text || "";
    $('#' + id).summernote({
        placeholder: 'Type something...',
        tabsize: 2,
        height: 500
    });
    $('#' + id).summernote('code', text);
}

function getSummernote(id) {
    return $('#' + id).summernote('code');
}
// END Summernote editor



// START initializeOrdersChart
function initializeOrdersChart(canvasId, labels, totals) {
    const canvas = document.getElementById(canvasId);

    if (!canvas) {
        console.warn(`Canvas element with id '${canvasId}' not found. Retrying in 100ms...`);
        setTimeout(() => initializeOrdersChart(canvasId, labels, totals), 100);
        return;
    }

    const ctx = canvas.getContext('2d');
    new Chart(ctx, {
        type: 'line', // 'line' ili 'bar'
        data: {
            labels: labels,
            datasets: [{
                label: 'Total Orders',
                data: totals,
                borderColor: 'rgba(75, 192, 192, 1)',
                backgroundColor: 'rgba(75, 192, 192, 0.5)',
                fill: true,
                tension: 0.4
            }]
        },
        options: {
            responsive: true,
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Datum'
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Total'
                    }
                }
            }
        }
    });
}
// END initializeOrdersChart