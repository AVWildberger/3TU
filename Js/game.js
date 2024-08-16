// Global variables
const game = document.getElementsByClassName('game')[0];
let turn = 'X';

// Generate a 3x3 grid of fields, each containing a 3x3 grid of cells
function generateGame() {
    let cellField = 0;
    let cellNumbInField = 0;

    // Create a 3x3 grid of fields
    for (let row = 0; row < 3; row++) {  
        // Create a row of fields     
        const rowDiv = document.createElement('div');
        rowDiv.classList.add('row');

        // Create a 3x3 grid of fields within each row
        for (let col = 0; col < 3; col++) {
            // Update variables for each field and reset cell number
            cellField++;
            cellNumbInField = 0;

            // Create a field
            const fieldDiv = document.createElement('div');
            fieldDiv.classList.add('field');

            // Create a 3x3 grid of cells within each field
            for (let innerRow = 0; innerRow < 3; innerRow++) {
                // Create a row of cells
                const innerRowDiv = document.createElement('div');
                innerRowDiv.classList.add('inner-row');

                // Create a 3x3 grid of cells within each row
                for (let innerCol = 0; innerCol < 3; innerCol++) {
                    // Update cell number
                    cellNumbInField++;

                    // Create a cell
                    const cellDiv = document.createElement('div');
                    cellDiv.classList.add('cell');
                
                    // Create a cell content
                    const cellContent = document.createElement('p');                    
                    const content = ' ';
                    cellContent.textContent = content;

                    // Add notation to cell
                    const myField = cellField;
                    const myCellNumber = cellNumbInField;

                    // Add event listener to cell
                    cellDiv.addEventListener('click', () => {
                        clickCell(myField, myCellNumber);
                    });
                
                    // Append cell content to cell
                    cellDiv.appendChild(cellContent);
                    innerRowDiv.appendChild(cellDiv);
                }
                // Append row of cells to field
                fieldDiv.appendChild(innerRowDiv);
            }
            // Append field to row
            rowDiv.appendChild(fieldDiv);
        }
        // Append row to game
        game.appendChild(rowDiv);
    }
}
// Get cell by field and cell number
function getCell(field, fieldCell) {
    const rowIndex = Math.floor((field-1)/3);
    const fieldInRowIndex = Math.floor((field-1)%3);
    const rowInFieldIndex = Math.floor((fieldCell-1)/3);
    const cellInFieldIndex = Math.floor((fieldCell-1)%3);

    const docRow = document.getElementsByClassName('row')[rowIndex];
    const docField = docRow.getElementsByClassName('field')[fieldInRowIndex];
    const docInnerRow = docField.getElementsByClassName('inner-row')[rowInFieldIndex];
    const docCell = docInnerRow.getElementsByClassName('cell')[cellInFieldIndex];

    return docCell;
}
// Convert indexed notation to algebraic notation
function indexedToNotation(row, col, char) {
    const a = (row - (row % 3)) + ((col - (col % 3)) / 3) + 1;
    const b = (row % 3) * 3 + ((col % 3) + 1);

    return `${char}${a}${b}`;
}
// Convert algebraic notation to indexed notation
function notationToIndexed(notation) {
    const field = notation.charAt(1) - 1;
    const cell = notation.charAt(2) - 1;

    const arrayRow = field - (field % 3) + ((cell - (cell % 3))/3);
    const arrayCol = (field % 3) * 3 + (cell % 3);

    return [arrayRow, arrayCol];
}
// Log cell as algebraic notation
function logCellAsNotation(row, col, content) {

    // console.log(`Row: ${row}, Col: ${col}`);
    console.log(indexedToNotation(row, col, content));
}
// Handle cell click
function clickCell(row, col) {
    console.log(row, col)
    const notation = `${turn}${row}${col}`;

    // Get cell and cell content
    const cellDiv = getCell(notation.charAt(1), notation.charAt(2));
    const cellContent = cellDiv.getElementsByTagName('p')[0];

    // Update cell content
    if (cellContent.textContent === ' ' || true) {
        cellContent.textContent = turn;

        // Log cell notations
        const indexed = notationToIndexed(notation);
        console.log(`Notation: ${notation} Reverse: [${indexed[0]}, ${indexed[1]}]`);

        // Update turn
        turn = turn === 'X' ? 'O' : 'X';
    }
}

generateGame();