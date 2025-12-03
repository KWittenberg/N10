let charts = {};
let isDisposing = false;

export function renderWeatherCharts(data) {
    console.log("✅ JS: renderWeatherCharts called with", data);

    // Spriječi višestruko pozivanje
    if (window._weatherChartsRendering) {
        console.log("⚠️ Charts already rendering, skipping...");
        return;
    }

    window._weatherChartsRendering = true;

    // Sačekajte da se DOM potpuno učita
    if (document.readyState !== 'complete') {
        document.addEventListener('DOMContentLoaded', () => {
            setTimeout(() => createAllCharts(data), 100);
        });
    } else {
        setTimeout(() => createAllCharts(data), 100);
    }
}

function createAllCharts(data) {
    try {
        // Prvo uništi stare chartove
        disposeCharts();

        const configs = [
            { id: "tempChart", label: "Temperature", value: data.temp, unit: "°C", min: -20, max: 40, color: "rgba(54, 162, 235, " },
            { id: "humidityChart", label: "Humidity", value: data.humidity, unit: "%", min: 0, max: 100, color: "rgba(255, 206, 86, " },
            { id: "pressureChart", label: "Pressure", value: data.pressure, unit: "hPa", min: 950, max: 1050, color: "rgba(75, 192, 192, " },
            { id: "windChart", label: "Wind Speed", value: data.wind, unit: "m/s", min: 0, max: 20, color: "rgba(255, 99, 132, " }
        ];

        // Kreiraj sve chartove
        configs.forEach(config => {
            createSingleChart(config);
        });

        console.log("✅ All charts created successfully");
    } catch (error) {
        console.error("❌ Error creating charts:", error);
    } finally {
        window._weatherChartsRendering = false;
    }
}

function createSingleChart(config) {
    const canvas = document.getElementById(config.id);
    if (!canvas) {
        console.warn(`⚠️ Canvas ${config.id} not found in DOM`);
        return;
    }

    // Postavi eksplicitne dimenzije
    canvas.style.width = '100%';
    canvas.style.height = '100px';
    canvas.width = canvas.offsetWidth;
    canvas.height = 100;

    // Provjeri da li već postoji chart
    if (charts[config.id]) {
        try {
            charts[config.id].destroy();
        } catch (e) {
            console.warn(`Failed to destroy existing chart ${config.id}:`, e);
        }
    }

    try {
        const ctx = canvas.getContext('2d');

        // Koristi jednostavnu konfiguraciju
        charts[config.id] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: [config.label],
                datasets: [{
                    data: [config.value],
                    backgroundColor: `${config.color}0.7)`,
                    borderColor: `${config.color}1)`,
                    borderWidth: 1,
                    barPercentage: 0.5,
                }]
            },
            options: {
                responsive: false, // VAŽNO: Postavi na false da Chart.js ne upravlja veličinom
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                return `${context.raw} ${config.unit}`;
                            }
                        }
                    },
                    title: {
                        display: true,
                        text: `${config.label}: ${config.value} ${config.unit}`,
                        font: { size: 12 }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        min: config.min,
                        max: config.max,
                        display: false // Sakrij y-os za jednostavniji prikaz
                    },
                    x: {
                        display: false // Sakrij x-os
                    }
                },
                animation: {
                    duration: 0 // Onemogući animaciju za brže renderovanje
                }
            }
        });

        console.log(`✅ Chart ${config.id} created with value ${config.value}`);
    } catch (error) {
        console.error(`❌ Error creating chart ${config.id}:`, error);
    }
}

export function disposeCharts() {
    if (isDisposing) return;

    isDisposing = true;
    console.log("🧹 Disposing all charts...");

    Object.keys(charts).forEach(key => {
        if (charts[key] && typeof charts[key].destroy === 'function') {
            try {
                charts[key].destroy();
            } catch (e) {
                console.warn(`Error destroying chart ${key}:`, e);
            }
        }
    });

    charts = {};
    isDisposing = false;

    // Resetiraj globalnu zastavicu
    window._weatherChartsRendering = false;
}

// Dodajte globalni handler za čišćenje
window.addEventListener('beforeunload', disposeCharts);