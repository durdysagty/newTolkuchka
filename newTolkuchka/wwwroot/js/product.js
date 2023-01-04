function getProduct(id) {
    const divs = $("div[id*='pr']")
    divs.each(function () {
        if (this.id === `pr${id}`)
            $(this).addClass('d-block').removeClass('d-none')
        else
            $(this).addClass('d-none').removeClass('d-block')
    })
}
function setImage(img, n) {
    let src = $(img).attr('src')
    src = src.replace(/-[0-4]/, `-${n}`)
    $(img).attr('src', src)
}
checkOrders()