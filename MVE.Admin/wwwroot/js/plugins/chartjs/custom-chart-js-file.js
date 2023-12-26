function initalizeChartLine(ylables, xlables, bookingData, userData, chartcontainerdiv) {
 
    Chart.defaults.global.defaultFontStyle = "normal";
    Chart.defaults.global.defaultFontSize = 15;
    Chart.defaults.global.defaultFontColor = '#000';
    var ctx = document.getElementById(chartcontainerdiv);

    var bookingDataset = {
        label: "Total Bookings",
        data: bookingData,
        lineTension: 0,
        fill: false,
        borderColor: '#5ca29e',
        borderWidth: 3
    };

    var userDataset = {
        label: "Total Users",
        data: userData,
        lineTension: 0,
        fill: false,
        borderColor: '#9005fa',
        borderWidth: 3
    };   

    var chrt = new Chart(ctx, {
        type: 'line',
        data: {
            labels: xlables,
            datasets: [bookingDataset, userDataset]
        },
        options: {
            responsive: true,
            scaleOverride: false,
            showTooltips: true,
            maintainAspectRatio: false,
            fontStyle: 'normal',
            tooltips: {
                enabled: true,
                mode: 'index', // Display multiple tooltips at the same index
                position: 'nearest',
                intersect: true
            },
            hover: {
                animationDuration: 0
            },
            legend: {
                display: true,
                position: 'top',
                labels: {
                    fontStyle: 'normal',
                    fontSize: 20,
                    display: true
                }
            }
        }
    });

}

function initalizeEarningChartLine(ylables, xlables, earningData, chartcontainerdiv) {
    Chart.defaults.global.defaultFontStyle = "normal";
    Chart.defaults.global.defaultFontSize = 15;
    Chart.defaults.global.defaultFontColor = '#000';
    var ctx = document.getElementById(chartcontainerdiv);

    var earningDataset = {
        label: "Total Earning",
        data: earningData,
        lineTension: 0,
        fill: false,
        borderColor: '#5ca29e',
        borderWidth: 3
    };

    var chrt = new Chart(ctx, {
        type: 'line',
        data: {
            labels: xlables,
            datasets: [earningDataset]
        },
        options: {
            responsive: true,
            scaleOverride: true,
            showTooltips: true,
            maintainAspectRatio: false,
            fontStyle: 'normal',
            tooltips: {
                enabled: true
            },
            hover: {
                animationDuration: 0
            },
            legend: {
                display: true,
                position: 'top',
                labels: {
                    fontStyle: 'normal',
                    fontSize: 20,
                    display: true
                }
            }
        }
    });

}