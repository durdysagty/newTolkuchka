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
const gallery = baguetteBox.run(".gallery");
function showGalery(i) {
    baguetteBox.show(0, gallery[i]);
}
function quantity(id, add) {
    let q = $(id).text()
    if (add)
        $(id).text(++q)
    else if (q > 1)
        $(id).text(--q)
}
function productOrder(id, quantityId) {
    const q = $(quantityId).text()
    order(id, parseInt(q))
}