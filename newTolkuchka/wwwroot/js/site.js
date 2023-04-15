document.cookie = `w=${window.innerWidth}; max-age=2592000; samesite=strict; secure; path=/`
const stringList = {
    add: 'в корзину',
    added: 'в корзине',
    currency: 'TMT',
    wrong: 'Что-то пошло не так! Попробуйте ещё раз или обратитесь к администрации сайта.'
}
if (window.location.hostname.includes('en')) {
    stringList.add = 'add to cart'
    stringList.added = 'added to cart'
    stringList.wrong = 'Something went wrong! Please try again or contact the website administrator.'
}
else if (window.location.hostname.includes('tm')) {
    stringList.add = 'sebete goş'
    stringList.added = 'sebetde'
    stringList.wrong = 'Bir zat telek boldy! Gaýtadan synanyşyň ýa-da web sahypa administratoryna ýüz tutmagyňyzy haýyş edýäris.'
}
function changeImage(id, ids) {
    ids.forEach(i => { $(i).addClass('d-none').removeClass('d-block') })
    $(id).addClass('d-block').removeClass('d-none')

}
let orders = JSON.parse(sessionStorage.getItem('orders'))
let scaled = JSON.parse(sessionStorage.getItem('scaled'))
function order(id, q = 1) {
    if (orders !== null) {
        const order = orders.find(o => o.id === parseInt(id))
        if (order !== undefined) {
            if (q === 1)
                order.quantity++
            else
                order.quantity = q
            orders[orders.indexOf(order)] = order
            sessionStorage.setItem('orders', JSON.stringify(orders))
        }
        else
            setOrder(id, q)
    }
    else
        setOrder(id, q)
}
let likes = JSON.parse(localStorage.getItem('likes'))
function like(id) {
    if (likes !== null) {
        const like = likes.find(l => l === parseInt(id))
        if (like === undefined) {
            likes.push(id)
            liked(id)
        }
        else {
            likes.splice(likes.indexOf(id), 1)
            unliked(id)
        }
    }
    else {
        likes = [id]
        liked(id)
    }
    localStorage.setItem('likes', JSON.stringify(likes))
}
function scale(id, reload = false) {
    const pid = parseInt(id)
    if (scaled === null)
        scaled = []
    if (scaled.find(s => s === pid) === undefined) {
        if (scaled.length < 4) {
            scaled.push(pid)
            scaler(id)
        }
    }
    else {
        scaled.splice(scaled.indexOf(pid), 1)
        unscaler(id)
    }
    const s = JSON.stringify(scaled)
    sessionStorage.setItem('scaled', s)
    document.cookie = `scaled=${s.replaceAll(',', '-')}; path=/`
    setS()
    if (reload)
        window.location.reload()
}
function setOrder(id, q = 1) {
    added(id)
    const order = {
        id: parseInt(id),
        quantity: q
    }
    if (orders !== null)
        orders.push(order)
    else
        orders = [order]
    sessionStorage.setItem('orders', JSON.stringify(orders))
    setQ()
}
// used to setQ by orders, used after page products are fully ready (e.g. filter.js setProducts)
function checkOrders() {
    if (orders !== undefined && orders !== null) {
        setQ()
        orders.forEach((o) => {
            added(o.id)
        })
    }
    if (likes !== undefined && likes !== null) {
        likes.forEach((l) => {
            liked(l)
        })
    }
    if (scaled !== undefined && scaled !== null) {
        setS()
        scaled.forEach((s) => {
            scaler(s)
        })
    }
}
function added(id) {
    const bId = `button[name='order${id}']`
    $(bId).each(function () {
        $(this).removeClass('btn-primary').addClass('btn-secondary')
        if ($(this).text() === stringList.add)
            $(this).html(stringList.added)
    })
}
function liked(id) {
    const bId = `button[name='like${id}']`
    $(bId).each(function () {
        $(this).removeClass('btn-primary').addClass('btn-secondary').addClass('text-danger')
    })
}
function unliked(id) {
    const bId = `button[name='like${id}']`
    $(bId).each(function () {
        $(this).addClass('btn-primary').removeClass('btn-secondary').removeClass('text-danger')
    })
}
function scaler(id) {
    const bId = `button[name='scale${id}']`
    $(bId).each(function () {
        $(this).removeClass('btn-primary').addClass('btn-secondary')
    })
}
function unscaler(id) {
    const bId = `button[name='scale${id}']`
    $(bId).each(function () {
        $(this).addClass('btn-primary').removeClass('btn-secondary')
    })
}
// set orders quantity to top corner of cart
function setQ() {
    $("span[name='q']").text(Object.keys(orders).length)
}
function setS() {
    $("span[name='s']").text(scaled.length)
}
if (navigator.userAgent.indexOf("Android") !== -1 && navigator.userAgent.indexOf("iPhone") !== -1 && navigator.userAgent.indexOf("iPad") !== -1) {
    $('#social').html(`<a href="instagram://user?username=tolkuchka.bar" rel="noreferrer noopener"><img width='77' height='77' style='height: 30px; width: auto' src="/ig.png" alt="instagram" /></a><a href="vk://vk.com/club114989678" rel="noreferrer noopener" class="ps-2"><img width='77''height='73' style='height: 32px; width: auto' src="/vk.png" alt="vk" /></a>`)
}
else {
    $('#social').html(`<a href="https://instagram.com/tolkuchka.bar" rel="noreferrer noopener" target="_blank"><img width='77' height='77' style='height: 30px; width: auto' src="/ig.png" alt="instagram" /></a><a href="https://vk.com/club114989678" class="ps-2" rel="noreferrer noopener" target="_blank"><img width='77''height='73' style='height: 32px; width: auto' src="/vk.png" alt="vk" /></a>`)
}