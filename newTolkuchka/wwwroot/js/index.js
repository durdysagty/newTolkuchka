async function getItems() {
    try {
        const response = await fetch(`/index`, {
            method: 'GET',
            credentials: 'include'
        })
        if (response.ok) {
            const result = await response.json()
            $("#fl").append(result.fl)
            $("#promo").append(result.p)
            $("#items").append(result.i)
            function autoplay(run) {
                clearInterval(interval)
                interval = setInterval(() => {
                    if (run && slider) {
                        slider.next()
                    }
                }, 3000)
            }
            const slider = new KeenSlider("#fl", {
                breakpoints: {
                    "(min-width: 320px)": {
                        slides: { perView: 3, spacing: 1 },
                    },
                    "(min-width: 428px)": {
                        slides: { perView: 4, spacing: 2 },
                    },
                    "(min-width: 576px)": {
                        slides: { perView: 5, spacing: 2 },
                    },
                    "(min-width: 768px)": {
                        slides: { perView: 6, spacing: 4 },
                    },
                    "(min-width: 992px)": {
                        slides: { perView: 7, spacing: 6 },
                    },
                    "(min-width: 1200px)": {
                        slides: { perView: 8, spacing: 8 },
                    },
                    "(min-width: 1400px)": {
                        slides: { perView: 9, spacing: 9 },
                    },
                    "(min-width: 1650px)": {
                        slides: { perView: 10, spacing: 2 },
                    }
                },
                slides: { perView: 2 },
                loop: true,
                defaultAnimation: {
                    duration: 2000
                    },
                dragStart: () => { autoplay(false) },
                dragEnd: () => { autoplay(true) }
            })
            var brSlider = document.getElementById("fl")
            brSlider.addEventListener("mouseover", () => {
                autoplay(false)
            })
            brSlider.addEventListener("mouseout", () => {
                autoplay(true)
            })
            autoplay(true)
        }
        checkOrders()
    }
    catch {
        console.log('!')
    }
}
getItems()