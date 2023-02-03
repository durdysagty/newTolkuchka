﻿async function getItems() {
    try {
        const response = await fetch(`/index`, {
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