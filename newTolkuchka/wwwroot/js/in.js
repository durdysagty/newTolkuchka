const r = {
    success: 0,
    fail: 1,
    new: 2
}
function inputLogin(e) {
    if (e.target.validity.typeMismatch)
        $('#wrongLog').html('Напишите правильный email!')
    else if (e.target.validity.valueMissing)
        $('#wrongLog').html('Вы не написали электронную почту!')
}
function invalidloginHandler(e) {
    e.preventDefault()
    $('#loginForm').addClass('was-validated')
}
let pin = []
let interval = null
async function loginHandler(e) {
    e.preventDefault()
    $('#pinWrong').html('')
    $('#pinText').html('')
    $('#logText').html('')
    let input = e.currentTarget[0]
    try {
        const response = await fetch('/login/userlogin', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(input.value)
        })
        if (response.ok) {
            const result = await response.json()
            if (result.result === r.success || result.result === r.new) {
                $('#timer').html(`5:00`)
                $('#pinEvoke').click()
                if (result.result === r.new)
                    $('#pinText').html(result.text)
                let ms = 300000
                if (interval !== null)
                    clearInterval(interval)
                interval = setInterval(() => {
                    ms = ms - 1000
                    if (ms === 0)
                        $('#closePin').click()
                    const t = new Date(ms)
                    const m = t.getMinutes()
                    const s = t.getSeconds()
                    $('#timer').html(`${m}:${s}`)
                }, 1000)
                pin = [result.data]
                $(`#0`).focus()
            }
            else if (result.result === r.fail)
                $('#logText').html(result.text)
            else
                $('#logText').html(stringList.wrong)
        }
        else
            $('#logText').html(stringList.wrong)
    }
    catch {
        $('#logText').html(stringList.wrong)
    }
}

async function forgotPassword(e) {
    e.preventDefault()
    try {
        const response = await fetch('/login/recovery', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(pin.join(''))
        })
        if (response.ok) {
            const result = await response.json()
            if (result.result === r.success) 
                $('#pinText').html(result.text)
            else if (result.result === r.fail)
                $('#pinWrong').html(result.text)
            else
                $('#pinWrong').html(stringList.wrong)
        }
        else
            $('#pinWrong').html(stringList.wrong)
    }
    catch {
        $('#pinWrong').html(stringList.wrong)
    }
}

async function inputPin(e) {
    let id = parseInt(e.target.id)
    pin.push(e.target.value)
    $(`#${id}`).attr('disabled', true)
    id++
    if (id < 4) {
        $(`#${id}`).removeAttr('disabled')
        $(`#${id}`).focus()
    }
    else {
        try {
            const response = await fetch('/login/userpin', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(pin.join(''))
            })
            if (response.ok) {
                const result = await response.json()
                if (result.result === r.success)
                    window.location.reload()
                else {
                    $('#pinWrong').html(result.text)
                    for (let i = 0; i < 4; i++) {
                        $(`#${i}`).val('').attr('disabled', true)
                    }
                    $(`#${0}`).removeAttr('disabled')
                    pin = pin.slice(0, 1)
                    // $(`#${0}`).focus()
                }

            }
            else
                $('#pinWrong').html(stringList.wrong)
        }
        catch {
            $('#pinWrong').html(stringList.wrong)
        }
    }
}