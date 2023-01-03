let count = 6
if (window.innerWidth < 1200 && window.innerWidth > 992)
    count = 8
else if (window.innerWidth < 351)
    count = 4
async function getItems() {
    try {
        const response = await fetch(`/index/?count=${count}`, {
            method: 'GET',
            credentials: 'include'
        })
        if (response.ok) {
            const result = await response.text()
            $("#items").append(result)
        }
    }
    catch {
        console.log('!')
    }
}
getItems()