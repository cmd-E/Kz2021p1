document.addEventListener('DOMContentLoaded', () => {
    let removeLinks = document.querySelectorAll('.rmFlight')
    removeLinks.forEach(link => {
        link.addEventListener('click', removeFlight)
    })
})

async function removeFlight(e) {
    if (e.target.classList.contains('disabled-link')) {
        return
    }
    e.target.classList.add('disabled-link')
    const flightId = parseInt(e.target.getAttribute('data-id'))
    const port = getCurrentPort()
    const deleteFlightUrl = `https://localhost:${port}/Airport/RemoveFlight?flightId=${flightId}`
    await fetch(deleteFlightUrl, {
        method: 'POST',
    }).then(response => {
        if (response.ok) {
            e.target.parentNode.parentNode.remove() // remove table row
        }
    })
}

function getCurrentPort() {
    return window.location.href.split('/')[2].split(':')[1]
}