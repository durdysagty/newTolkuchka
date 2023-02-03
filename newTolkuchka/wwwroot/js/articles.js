let h
let page
async function getItems() {
    let data
    try {
        const response = await fetch(`/articlesbh?headingId=${h}&page=${page}`, {
            method: 'GET',
            credentials: 'include'
        })
        if (response.ok) {
            const result = await response.json()
            data = result
            if (data.articles.length > 0) {
                $("#articles").html(result.articles)
                $("#pagination").text('')
                $("#pagination").append(`${data.pagination}`)
                $("#pagination").append(`<i class="fas fa-angle-double-left ps-2" role="button" onclick="setPage(0)"></i>`)
                $("#pagination").append(`<i class="fas fa-angle-left ps-1" role="button" onclick="setPage(null, 0)"></i>`)
                $("#pagination").append(`<i class="fas fa-angle-right ps-1" role="button" onclick="setPage(null, ${data.lastPage})"></i>`)
                $("#pagination").append(`<i class="fas fa-angle-double-right ps-1" role="button" onclick="setPage(${data.lastPage})"></i>`)
            }
            else
                noArticle(data.noArticle)            
        }
    }
    catch {
        console.log('!')
    }
}
getItems()
function setHeadingId(id) {
    h = id
    page = 0;
    getItems()
}
function setPage(e, lastPage) {
    if (e === null) {
        if (lastPage > 0) {
            let i = page
            if (++i <= lastPage) {
                page++
                getItems()
            }
        }
        else {
            if (page != 0) {
                page--
                getItems()
            }
        }
    }
    else if (page !== e) {
        page = e
        getItems()
    }
}
function noArticle(text) {
    $("#articles").text('')
    $("#articles").html(`<div><div class="row justify-content-center justify-content-md-start"><div style="height: 50vh" class="d-flex justify-content-center align-items-center"><p class="text-center">${text}</p></div></div></div>`)
}