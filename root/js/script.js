const allowedMoves = new Set(['ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown']);
var boardState = [];

document.addEventListener('DOMContentLoaded', () => {
    initializeBoard();
    document.addEventListener('keydown', handleKeyPress);
});

function handleKeyPress(event) {
    if (!allowedMoves.has(event.key)) {
        return;
    }
    
    switch(event.key) {
        case 'ArrowLeft':
            moveLeft();
            break;
        case 'ArrowRight':
            moveRight();
            break;
        case 'ArrowUp':
            moveUp();
            break;
        case 'ArrowDown':
            moveDown();
            break;
    }

    spawnRandomCells(1);
}

function initializeBoard() {
    loadCells();
    spawnRandomCells(2);
}

function spawnRandomCells(numberToCreate){
    for(var i = 0; i < numberToCreate; i++) {
        var emptyCells = getEmptyCells();

        if(emptyCells.length > 0) {
            var cellValue = Math.random() < 0.9 ? 2 : 4;
            var cell = emptyCells[Math.floor(Math.random() * emptyCells.length)];
            var childElement = document.createElement('div');
            childElement.classList.add('tile-number');
            childElement.dataset.value = cellValue;
            childElement.textContent = cellValue;
            cell.appendChild(childElement);
            childElement.dataset.value = cellValue;
        }      
    }
}

function loadCells(){
    // Initialize board as a 4x4 2D array
    boardState = [[null, null, null, null], 
             [null, null, null, null], 
             [null, null, null, null], 
             [null, null, null, null]];
    
    // Get all tile elements
    var tiles = document.querySelectorAll('.tile');
    
    // Place each tile in the correct position based on data-row and data-column
    tiles.forEach(tile => {
        var row = parseInt(tile.dataset.row);
        var column = parseInt(tile.dataset.column);
        boardState[row][column] = tile;
    });
}

function getEmptyCells(){ return Array.from(document.querySelectorAll('.tile')).filter(t => t.childElementCount == 0); }

function moveLeft() {
    for(var row = 0; row < boardState.length; row++) {
        for(var column = 1; column < boardState[row].length; column++) {
            if(!boardState[row][column].hasChildNodes() || !boardState[row][column - 1].hasChildNodes()) {
                continue;
            }
            
            var currentCell = boardState[row][column].getElementsByTagName('div')[0];
            var currentCellValue = parseInt(currentCell.dataset.value);

            var leftNeighborCell = boardState[row][column - 1].getElementsByTagName('div')[0];
            var leftNeighborCellValue = parseInt(leftNeighborCell.dataset.value);

            if(currentCellValue === leftNeighborCellValue) {
                var newCellValue = currentCellValue * 2;
                leftNeighborCell.dataset.value = newCellValue;
                leftNeighborCell.textContent = newCellValue;
                currentCell.remove();
            }
            else{
                
            }
        }
    }
}

function moveRight() {

}

function moveUp() {

}

function moveDown() {

}