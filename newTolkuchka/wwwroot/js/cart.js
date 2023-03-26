const stringListCart = {
    name: 'Наименование',
    price: 'Цена',
    quantity: 'Количество',
    amount: 'Сумма',
    delivery: 'Доставка',
    summary: 'Итого',
    empty: 'Ваша корзина пуста',
    freeOne: 'БЕСПЛАТНО',
    //emptyInput: 'Это поле не должно быть пустым!',
    //numbersOnly: 'Пожалуйста, пишите только цифры!',
    deliveryPrice: 20,
    deliveryFree: 500
}
if (window.location.hostname.includes('en')) {
    stringListCart.name = 'Product'
    stringListCart.price = 'Price'
    stringListCart.quantity = 'Quantity'
    stringListCart.amount = 'Amount'
    stringListCart.delivery = 'Delivery'
    stringListCart.summary = 'Summary'
    stringListCart.empty = 'Your cart is empty'
    stringList.freeOne = 'FOR FREE'
    //stringListCart.emptyInput = 'This field must not be empty!'
    //stringListCart.numbersOnly = 'Numbers only!'
}
else if (window.location.hostname.includes('tm')) {
    stringListCart.name = 'Haryt'
    stringListCart.price = 'Baha'
    stringListCart.quantity = 'Sany'
    stringListCart.amount = 'Jemi'
    stringListCart.delivery = 'Eltmesi'
    stringListCart.summary = 'Jemi netijesi'
    stringListCart.empty = 'Siziň sepetiňiz boş'
    stringList.freeOne = 'MUGT'
    //stringListCart.emptyInput = 'Bu meýdança boş bolmaly däl!'
    //stringListCart.numbersOnly = 'Diňe san ýazmaly!'
}
let sum = 0
let delivery = 0
async function getOrderProducts() {
    // console.log(orders)
    try {
        const response = await fetch('/cart/data', {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(orders)
        })
        if (response.ok) {
            const result = await response.json()
            // console.log(result)
            if (result.noOrders)
                setOrderResult(stringListCart.empty)
            else {
                $('#delivery').removeClass('d-none')
                orders = result.orders
                stringListCart.deliveryFree = result.deliveryFree
                stringListCart.deliveryPrice = result.deliveryPrice
                $('#cart').append(`<table class="table table-hover table-sm"><thead><tr><th></th><th>${stringListCart.name}</th><th>${stringListCart.price}, ${stringList.currency}</th><th class='text-center'>${stringListCart.quantity}</th><th>${stringListCart.amount}, ${stringList.currency}</th><th></th></tr></thead><tbody id="cartBody"></tbody></table>`)
                result.orders.forEach(o => {
                    const tr = $(`<tr><td class='align-middle p-0'><a href='/product/${o.id}'><img class='img-fluid' style='max-height: 50px' src='${o.image}' /></a></td><td class='align-middle'><a href='/product/${o.id}'>${o.productName}</a> ${o.freeQuantity !== null ? `<span id=${'free' + o.id} ${o.quantity >= o.freeQuantity ? 'class=\"d-block\"' : 'class=\"d-none\"'}>+1  ${stringListCart.freeOne}</span>` : ''} ${o.freeProductQuantity !== null ? `<span id=${'freeQ' + o.id} ${o.quantity >= o.freeProductQuantity ? 'class=\"d-block\"' : 'class=\"d-none\"'}>+ ${o.freeProductName} - ${stringListCart.freeOne}</span>` : ''}  ${o.setId !== null ? `<span>+ ${o.setFreeProductName} - ${stringListCart.freeOne}</span>` : ''}</td><td class='align-middle'  id='${'p' + o.id}'>${o.price}</td><td class='align-middle text-center'>${o.subjected ? '' : `<span role="button" class='pointer px-2' onclick='quantity(${o.id}, false)'>-</span><span id='${'q' + o.id}' > ${o.quantity}</span > <span role="button" class='pointer px-2' onclick='quantity(${o.id}, true)'>+</span>`}</td><td class='align-middle'><span id='${' s' + o.id}'>${o.amount}</span></td><td class='align-middle'><button class='btn btn-primary py-1 px-3' onclick='removeOrder(${o.id})'>X</button></td></tr> `)
                    $('#cartBody').append(tr)
                    sum = ((sum * 10) + (((o.price * 10 * o.quantity * 10) / 100) * 10)) / 10
                })
                if (sum < stringListCart.deliveryFree) {
                    delivery = stringListCart.deliveryPrice
                    sum = sum + stringListCart.deliveryPrice
                }
                const summary = $(`<tr><th></th><th></th><th></th><th class='text-right'>${stringListCart.delivery}: </th><th><span id='delivery'>${delivery}</span> тмт</th><th></th></tr><tr><th></th><th></th><th></th><th class='text-right'>${stringListCart.summary}: </th><th><span id='sum'>${sum}</span> ${stringList.currency}</th><th></th></tr>`)
                $('#cartBody').append(summary)
            }
        }
        else
            setOrderResult(stringList.wrong)
    }
    catch {
        setOrderResult(stringList.wrong)
    }
}
function quantity(id, add) {
    const order = orders.find(o => o.id === id)
    if (add)
        order.quantity++
    else
        if (order.quantity > 1)
            order.quantity--
    if (order.discountQuantity !== null) {
        if (order.quantity >= order.discountQuantity)
            order.price = order.quantityPrice
        else
            order.price = order.regularPrice
        $(`#${'p' + id} `).text(order.price)
    }
    if (order.freeQuantity !== null) {
        if (order.quantity >= order.freeQuantity)
            $(`#${'free' + id} `).addClass('d-block').removeClass('d-none')
        else
            $(`#${'free' + id} `).addClass('d-none').removeClass('d-block')
    }
    if (order.freeProductQuantity !== null) {
        if (order.quantity >= order.freeProductQuantity)
            $(`#${'freeQ' + id} `).addClass('d-block').removeClass('d-none')
        else
            $(`#${'freeQ' + id} `).addClass('d-none').removeClass('d-block')
    }
    orders[orders.indexOf(order)] = order
    sessionStorage.setItem('orders', JSON.stringify(orders))
    $(`#${'q' + id} `).text(order.quantity)
    let sum = (order.price * 10 * order.quantity * 10) / 100
    $(`#${'s' + id} `).text(sum)
    sum = 0
    orders.forEach(o => {
        sum = ((sum * 10) + (((o.price * 10 * o.quantity * 10) / 100) * 10)) / 10
    })
    let delivery = 0
    if (sum < stringListCart.deliveryFree) {
        delivery = stringListCart.deliveryPrice
        sum = sum + delivery
    }
    $(`#delivery`).text(delivery)
    $(`#sum`).text(sum)
}
function removeOrder(o) {
    // const orders = JSON.parse(sessionStorage.getItem('orders'))
    if (orders !== null) {
        orders.splice(orders.indexOf(o), 1)
        if (orders.length > 0)
            sessionStorage.setItem('orders', JSON.stringify(orders))
        else
            sessionStorage.removeItem('orders')
    }
    window.location.reload()
}
function inputOrder(e) {
    if (e.target.name === 'phone-number')
        if (/^\d+$|^$|^\+?$|^\+?\d+$/.test(e.target.value))
            e.target.value = e.target.value
}
function invalidOrdernHandler(e) {
    e.preventDefault()
    $('#orderForm').addClass('was-validated')
}
async function orderHandler(e) {
    e.preventDefault()
    const inputs = $("input[id*='order']")
    let deliveryData = {
        name: '',
        email: '',
        phone: '',
        address: ''
    }
    inputs.each(function () {
        deliveryData[$(this).attr('name')] = $(this).val()
    })
    //console.log(orders)
    const formData = new FormData()
    formData.append('orders', JSON.stringify(orders))
    for (let key in deliveryData) {
        formData.append(key, deliveryData[key]);
    }
    try {
        const response = await fetch('/order', {
            method: 'POST',
            credentials: 'include',
            body: formData
        })
        if (response.ok) {
            const result = await response.json()
            if (result.noOrders)
                setOrderResult(stringListCart.empty)
            else
                setOrderResult(result.success)
            $('#delivery').addClass('d-none')
            sessionStorage.removeItem('orders')
            orders = []
            setQ()
        }
        else
            setOrderResult(stringList.wrong)
    }
    catch {
        console.log(stringList.wrong)
    }
}
function setOrderResult(text) {
    $('#cart').html(`<div class="justify-content-center align-items-center"><p class="text-center">${text}</p></div>`)
}
getOrderProducts()