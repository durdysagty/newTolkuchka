function toLcalTime() {
    $("td[name='psdate']").each(function () {
        const l = new Date($(this).text()).toLocaleString()
        $(this).text(l)
    })
} 
function inputAccount(e) {
    if (e.target.name === 'phone')
        if (/^\d+$|^$|^\+?$|^\+?\d+$/.test(e.target.value))
            e.target.value = e.target.value
    if (e.target.name === 'pin') {
        if (!/^\d+$/.test(e.target.value))
            e.target.value = null
        if (e.target.value.length > 4)
            e.target.value = e.target.value.slice(0,4)
    }
}
function invalidAccountHandler(e) {
    e.preventDefault()
    $('#userForm').addClass('was-validated')
}
toLcalTime()
async function logout() {
    const response = await fetch('/login/logout', {
        method: 'GET'
    })
    if (response.ok) {
        location.reload(true)
    }
    else
        location.reload(true)
}