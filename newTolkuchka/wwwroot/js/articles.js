let h;
async function getItems() {
    try {
        const response = await fetch(`/articlesbh?headingId=${h}`, {
            method: 'GET',
            credentials: 'include'
        })
        if (response.ok) {
            const result = await response.text()
            $("#articles").html(result)
        }
    }
    catch {
        console.log('!')
    }
}
getItems()
function setHeadingId(id) {
    h = id
    getItems()
}