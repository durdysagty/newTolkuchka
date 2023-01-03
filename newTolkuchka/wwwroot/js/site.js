document.cookie = `w=${window.innerWidth}; max-age=2592000; samesite=strict; secure; path=/`
const stringList = {
    // add: 'в корзину',
    // added: 'в корзине',
    currency: 'TMT',
    wrong: 'Что-то пошло не так! Попробуйте ещё раз или обратитесь к администрации сайта.'
}
if (window.location.hostname.includes('en')) {
    // stringList.add = 'add to cart'
    stringList.added = 'added to cart'
    stringList.wrong = 'Something went wrong! Please try again or contact the website administrator.'
}
else if (window.location.hostname.includes('tm')) {
    // stringList.add = 'sebete goş'
    stringList.added = 'sebetde'
    stringList.wrong = 'Bir zat telek boldy! Gaýtadan synanyşyň ýa-da web sahypa administratoryna ýüz tutmagyňyzy haýyş edýäris.'
}
let orders = JSON.parse(sessionStorage.getItem('orders'))
function order(id) {
    if (orders !== null) {
        const order = orders.find(o => o.id === parseInt(id))
        if (order !== undefined) {
            order.quantity++
            orders[orders.indexOf(order)] = order
            sessionStorage.setItem('orders', JSON.stringify(orders))
        }
        else
            setOrder(id)
    }
    else
        setOrder(id)
}
function setOrder(id) {
    added(id)
    const order = {
        id: parseInt(id),
        quantity: 1
    }
    if (orders !== null)
        orders.push(order)
    else
        orders = [order]
    sessionStorage.setItem('orders', JSON.stringify(orders))
    setQ()
}
function checkOrders() {
    if (orders !== undefined && orders !== null) {
        setQ()
        orders.forEach((o) => {
            added(o.id)
        })
    }
}
function added(id) {
    const bId = `button[name='order${id}']`
    $(bId).each(function () {
        $(this).removeClass('btn-primary').addClass('btn-secondary')
    })
}
// set orders quantity to top corner of cart
function setQ() {
    $("span[name='q']").text(Object.keys(orders).length)
}
checkOrders()
if (navigator.userAgent.indexOf("Android") !== -1 && navigator.userAgent.indexOf("iPhone") !== -1 && navigator.userAgent.indexOf("iPad") !== -1) {
    $('#social').html(`<a href="instagram://user?username=tolkuchka.bar" rel="noreferrer noopener"><img style='height: 30px' src="/ig.png" alt="instagram" /></a><a href="vk://vk.com/club114989678" rel="noreferrer noopener" class="ps-2"><img style='height: 32px' src="/vk.png" alt="vk" /></a>`)
}
else {
    $('#social').html(`<a href="https://instagram.com/tolkuchka.bar" rel="noreferrer noopener" target="_blank"><img style='height: 30px' src="/ig.png" alt="instagram" /></a><a href="https://vk.com/club114989678" class="ps-2" rel="noreferrer noopener" target="_blank"><img style='height: 32px' src="/vk.png" alt="vk" /></a>`)
}