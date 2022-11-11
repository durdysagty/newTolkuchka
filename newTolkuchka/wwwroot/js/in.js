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
async function loginHandler(e) {
    e.preventDefault()
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
                setInterval(() => {
                    ms = ms - 1000
                    if (ms === 0)
                        $('#closePin').click()
                    const t = new Date(ms)
                    const m = t.getMinutes()
                    const s = t.getSeconds()
                    $('#timer').html(`${m}:${s}`)
                }, 1000)
                pin.push(result.data)
                $(`#${0}`).focus()
            }
            else if (result.result === r.fail)
                $('#logText').html(result.data)
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