let count = 6
if (window.innerWidth < 1200 && window.innerWidth > 768)
    count = 4
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
        checkOrders()
    }
    catch {
        console.log('!')
    }
}
getItems()