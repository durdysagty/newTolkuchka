const mobile = 992
const searchObj = {
    t: [],
    b: [],
    v: [],
    min: '',
    max: '',
    minp: '',
    maxp: '',
    sort: 0,
    page: 0
}
$(window).on("popstate", function () {
    const search = new URLSearchParams(window.location.search.substring(1))
    const priceFilter = "<input type=\"text\" class=\"jsRange\" name=\"m\" id=\"ran\" />"
    if (searchObj.min !== '') {
        $("#range").html(priceFilter)
        $(".jsRange").ionRangeSlider({
            skin: "flat",
            type: "int",
            min: searchObj.min,
            max: searchObj.max,
            from: search.get('minp') !== null ? search.get('minp') : null,
            to: search.get('maxp') !== null ? search.get('maxp') : null,
            from_fixed: false,
            to_fixed: false,
            onFinish: setFilters
        })
    }
    prepareSearchObj()
    const inputs = $("input[name*='fil']")
    inputs.each(function () {
        if (this.name == 'filt') {
            const i = searchObj.t.includes(this.value)
            this.checked = i
        }
        else if (this.name == 'filb') {
            const i = searchObj.b.includes(this.value)
            this.checked = i
        }
        else {
            const i = searchObj.v.includes(this.value)
            this.checked = i
            const img = $(`img[name="img${this.value}"]`)
            if (img !== null)
                if (i)
                    img.addClass('border border-black')
                else
                    img.removeClass('border border-black')
        }
    })
    const sorts = $("[name*='sort']")
    sorts.each(function (i) {
        if (Math.abs(searchObj.sort) === i)
            $(this).replaceWith(`<strong class="ms-2" role="button" onclick="setSort(${i})">` + $(this).text() + "</strong>")
        else
            $(this).replaceWith(`<p class="ms-2" role="button" onclick="setSort(${i})">` + $(this).text() + "</p>")
    })
    setProducts(true)
})
function prepareSearchObj() {
    const search = new URLSearchParams(window.location.search.substring(1))
    const entries = search.entries()
    const tids = []
    const bids = []
    const vids = []
    for (const [key, value] of entries) {
        if (key === 't')
            tids.push(value)
        else if (key === 'b')
            bids.push(value)
        else if (key === 'v')
            vids.push(value)
        else
            searchObj[key] = value
    }
    if (search.get('sort') === null)
        searchObj.sort = 0
    if (search.get('page') === null)
        searchObj.page = 0
    searchObj.t = tids
    searchObj.b = bids
    searchObj.v = vids
}
async function setProducts(productsOnly) {
    const path = window.location.pathname.split('/')
    const search = window.location.search.substring(1)
    if (path[1] === 'liked') {
        path[2] = localStorage.getItem('likes')
    }
    let data
    try {
        const response = await fetch(`/products/?model=${path[1]}&id=${path[2]}&productsOnly=${productsOnly}&${search}`, {
            method: 'GET',
            credentials: 'include'
        })
        if (response.ok) {
            const result = await response.json()
            data = result
        }
    }
    catch {
        console.log('!')
    }
    if (data.products.length > 0) {
        if (!productsOnly) {
            if (data.min !== data.max) {
                searchObj.min = data.min
                searchObj.max = data.max
                const priceFilter = "<input type=\"text\" class=\"jsRange\" name=\"m\" id=\"ran\" />"
                if (window.innerWidth > mobile)
                    $("#range").html(priceFilter)
                else
                    $("#slide-range").html(priceFilter)
                $(".jsRange").ionRangeSlider({
                    skin: "flat",
                    type: "int",
                    min: data.min,
                    max: data.max,
                    from: searchObj.minp !== '' ? searchObj.minp : null,
                    to: searchObj.maxp !== '' ? searchObj.maxp : null,
                    from_fixed: false,
                    to_fixed: false,
                    onFinish: setFilters
                })
            }
        }
        // types
        if (data.types.types !== null && data.types.types.length > 1) {
            $("#types").append(`<strong>${data.types.name}</strong><div id="typesList" class="scList"></div>`)
            //if (window.innerWidth > mobile)
            //    $("#types").append(`<strong>${data.types.name}</strong>`)
            //else
            //    $("#slide-filters").append(`<strong>${data.types.name}</strong>`)
            const types = data.types.types.map(t => {
                let id = `types${t.id}`
                const i = searchObj.t.includes(`${t.id}`)
                return `<div class="form-check d-flex"><input class="form-check-input" name="filt" ${i ? `checked` : null} onclick="setFilters()" type="checkbox" value="${t.id}" id="${id}" /><label class="form-check-label align-self-end" for="${id}">${t.name}</label></div>`
            })
            types.forEach(t => $("#typesList").append(t))
            //if (window.innerWidth > mobile)
            //    types.forEach(t => $("#filters").append(t))
            //else
            //    types.forEach(t => $("#slide-filters").append(t))
        }
        // brands
        if (data.brands.brands !== null && data.brands.brands.length > 1) {
            $("#brands").append(`<strong>${data.brands.name}</strong><div id="brandsList" class="scList"></div>`)
            //if (window.innerWidth > mobile)
            //    $("#filters").append(`<strong>${data.brands.name}</strong><div id="brands"></div>`)
            //else
            //    $("#slide-filters").append(`<strong>${data.brands.name}</strong><div id="brands"></div>`)
            const brands = data.brands.brands.map(b => {
                let id = `brands${b.id}`
                const i = searchObj.b.includes(`${b.id}`)
                return `<div class="form-check d-flex"><input class="form-check-input" name="filb" ${i ? `checked` : null} onclick="setFilters()" type="checkbox" value="${b.id}" id="${id}" /><label class="form-check-label align-self-end" for="${id}">${b.name}</label></div>`
            })
            brands.forEach(b => $("#brandsList").append(b))
        }
        // filters
        if (data.filters !== null) {
            const filters = data.filters.map(f => {
                const filterValues = f.isImaged ?
                    f.filterValues.map(fv => {
                        const id = `filVal${fv.id}`
                        const i = searchObj.v.includes(`${f.id},${fv.id}`)
                        return `<span class="my-2"><input name="filv" ${i ? `checked` : null} class="d-none" onclick="setFilters()" type="checkbox" value="${f.id},${fv.id}" id="${id}" /><picture><source type="image/webp" srcset="${fv.image}webp?v=${fv.imageVersion}"><source type="image/jpeg" srcset="${fv.image}jpg?v=${fv.imageVersion}"><img name="img${f.id},${fv.id}" width="25" height="25" style="width: 25px; height: auto; padding: 1px" role="button" onclick="clickInput(event, ${id})" title="${fv.name}" alt="${fv.name}" src="${fv.image}jpg?v=${fv.imageVersion}" class="my-1 border-1 ${i ? `border border-black` : null}" /></picture></span>`
                    }) :
                    f.filterValues.map(fv => {
                        const id = `filVal${fv.id}`
                        const i = searchObj.v.includes(`${f.id},${fv.id}`)
                        return `<div class="form-check d-flex"><input class="form-check-input" name="filv" ${i ? `checked` : null} onclick="setFilters()" type="checkbox" value="${f.id},${fv.id}" id="${id}" /><label class="form-check-label align-self-end" for="${id}">${fv.name}</label></div>`
                    })
                let filter = `<div><strong>${f.name}</strong></div>`
                filterValues.forEach(fv => filter += fv)
                return filter
            })
            $("#filters").html('')
            filters.forEach(b => $("#filters").append(b))
            //if (window.innerWidth > mobile)
            //    filters.forEach(b => $("#filters").append(b))
            //else
            //    filters.forEach(b => $("#slide-filters").append(b))
        }
        else if ((path[1] === 'brand' || path[1] === 'search') && data.filters === null) {
            $("#filters").html('')
        }
        const sortby = data.sort.map((s, i) => {
            if (Math.abs(searchObj.sort) === i)
                return `<strong class="ms-2" role="button" name="sort" onclick="setSort(${i})">${s}</strong>`
            else
                return `<p class="ms-2" role="button" name="sort" onclick="setSort(${i})">${s}</p>`
        })
        $("#sortby").text('')
        sortby.forEach(s => $("#sortby").append(s))
        if (data.products.length > 0) {
            $("#products").text('')
            data.products.forEach(p => $("#products").append(p))
            // pagination
            $("#pagination").text('')
            $("#pagination").append(`${data.pagination}`)
            $("#pagination").append(`${data.buttons}`)
            checkOrders()
        }
        else
            noProduct(data.noProduct)
    }
    else
        noProduct(data.noProduct)
}
function noProduct(text) {
    $("#products").text('')
    $("#products").html(`<div><div class="row justify-content-center justify-content-md-start"><div style="height: 50vh" class="d-flex justify-content-center align-items-center"><p class="text-center">${text}</p></div></div></div>`)
}
function clickInput(e, id) {
    id.click()
    if (id.checked)
        $(e.target).addClass('border border-black')
    else
        $(e.target).removeClass('border border-black')
}
function setSort(e) {
    if (e === 0) {
        if (searchObj.sort !== 0) {
            searchObj.sort = 0
            setFilters()
        }
    }
    else {
        if (e === 1)
            searchObj.sort === 1 ? searchObj.sort = -1 : searchObj.sort = 1
        else
            searchObj.sort === 2 ? searchObj.sort = -2 : searchObj.sort = 2
        setFilters()
    }
}
function setPage(e, lastPage) {
    if (e === null) {
        if (lastPage > 0) {
            let i = searchObj.page
            if (++i <= lastPage) {
                searchObj.page++
                setFilters()
            }
        }
        else {
            if (searchObj.page != 0) {
                searchObj.page--
                setFilters()
            }
        }
    }
    else if (searchObj.page !== e) {
        searchObj.page = e
        setFilters()
    }
}
function setFilters() {
    const inputs = $("input[name*='fil']")
    const tids = []
    const bids = []
    const vids = []
    inputs.each(function () {
        if (this.checked) {
            if (this.name == 'filt')
                tids.push(this.value)
            else if (this.name == 'filb')
                bids.push(this.value)
            else
                vids.push(this.value)
        }
    })
    searchObj.t = tids
    searchObj.b = bids
    searchObj.v = vids
    const minMaxValues = $("input[name='m']").val()
    if (minMaxValues !== undefined) {
        const minMax = minMaxValues.split(';')
        searchObj.minp = minMax[0]
        searchObj.maxp = minMax[1]
    }
    const search = $.param(searchObj, true)
    history.pushState(null, null, `${window.location.pathname}?${search}`)
    setProducts(true)
}
//function changeImage(img, btn, id) {
//    let src = $(img).attr('src')
//    src = src.replace(/[0-9]+-/, `${id}-`)
//    $(img).attr('src', src)
//    let btnName = $(btn).attr('name')
//    btnName = btnName.replace(/[0-9]+/, `${id}`)
//    $(btn).attr('name', btnName)
//}
//function orderByImage(img) {
//    const src = $(img).attr('src')
//    const number = /[0-9]+-/.exec(src)
//    const id = parseInt(number)
//    console.log(src)
//    console.log(number[0])
//    console.log(id)
//    order(id)
//}
setProducts(false)
prepareSearchObj()