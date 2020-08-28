function confirmDelete(id, isTrue){
    var deleteSpan = document.getElementById(`deleteSpan_${id}`)
    var confirmDeleteSpan = document.getElementById(`confirmDeleteSpan_${id}`)

    if (isTrue) {
        deleteSpan.style.display = "none"
        confirmDeleteSpan.style.display = "inline"
    } else {
        deleteSpan.style.display = "inline"
        confirmDeleteSpan.style.display = "none"
    }
}
