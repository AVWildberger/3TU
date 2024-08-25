// Global variables
const game = document.getElementsByClassName('game')[0];
const ws = new WebSocket('ws://localhost:8080');
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
// Get field by field number
function getField(field) {
    return document.getElementsByClassName('field')[field-1];
}
// Get cell by field and cell number
function getCell(field, fieldCell) {
    return getField(field).getElementsByClassName('cell')[fieldCell-1];
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
    const notation = `${turn}${row}${col}`;

    // Get cell and cell content
    const cellDiv = getCell(notation.charAt(1), notation.charAt(2));
    const cellContent = cellDiv.getElementsByTagName('p')[0];

    // Attempt to place a field
    const packet = `PLACE?${notation}`; 
    ws.send(packet);
}
// Update cell content and highlight next field
function updateCell(notation, nextField) {

    // Convert next field to number if neccessary
    let nfAsNumb = nextField;
    if (typeof nextField === 'string') {
        nfAsNumb = parseInt(nextField);
    }

    // Get cell and cell content
    const cellDiv = getCell(notation.charAt(1), notation.charAt(2));
    const cellContent = cellDiv.getElementsByTagName('p')[0];

    // Update cell content
    cellContent.textContent = notation.charAt(0);
    if (notation.charAt(0) === 'X') {
        cellContent.classList.add('x');
    } else {
        cellContent.classList.add('o');
    }
    // Update turn
    turn = turn === 'X' ? 'O' : 'X';

    // Highlight next field

    if (nfAsNumb === 0) {
        const packet = 'WIN';
        ws.send(packet);
    } else {       
        highlightField(nfAsNumb);
    }
}
// Mark won fields
function markWonFields(fields) {
    // fields are input as a string like this: XO_XXOO__

    for (let i = 0; i < fields.length; i++) {
        const field = getField(i+1);

        if (fields.charAt(i) === 'X') {
            field.classList.add('won-x');
            field.classList.remove('won-o');
            field.classList.remove('tie');
        } else if (fields.charAt(i) === 'O') {
            field.classList.remove('won-x');
            field.classList.add('won-o');
            field.classList.remove('tie');
        } else {
            field.classList.remove('won-x');
            field.classList.remove('won-o');
            if (fields.charAt(i) === 'T') {
                field.classList.add('tie');
            } else {
                field.classList.remove('tie');
            }
        }
    }
}
// Highlights the fields to play to
function highlightFields(fields) {
    // fields are input as a string like this: XO_XXOO__

    markWonFields(fields);

    for (let i = 0; i < fields.length; i++) {
        const field = getField(i+1);

        if (field.classList.contains('won-x') || field.classList.contains('won-o') || field.classList.contains('tie')) {
            field.classList.remove('highlight');
        } else {
            field.classList.add('highlight');
        }
    }
}
// Highlights a single field
function highlightField(field) {
    for (let i = 1; i <= 9; i++) {
        const fieldDoc = getField(i);

        if (i === field) {
            fieldDoc.classList.add('highlight');
        } else {
            fieldDoc.classList.remove('highlight');
        }
    }
}
// Sync board with server
function syncBoardData(data) {
    const cellStates = data[0]
    
    for (let i = 0; i < cellStates.length; i++) {
        const cellInfo = cellStates[i];

        const row = Math.floor(i / 9);
        const col = i % 9;

        const notation = indexedToNotation(row, col, cellInfo[0])

        const cellDiv = getCell(notation.charAt(1), notation.charAt(2));
        const cellContent = cellDiv.getElementsByTagName('p')[0];

        if (cellInfo[0] === 'X') {
            cellContent.textContent = 'X';
            cellContent.classList.add('x');
            cellContent.classList.remove('o');
        } else if (cellInfo[0] === 'O') {
            cellContent.textContent = 'O';
            cellContent.classList.add('o');
            cellContent.classList.remove('x');
        } else {
            cellContent.textContent = ' ';
            cellContent.classList.remove('x');
            cellContent.classList.remove('o');
        }
    }

    turn = data[2].charAt(0);
    
    if (data[3].charAt(0) - '0' === 0) {
        highlightFields(data[1]);
    } else {
        markWonFields(data[1]);
        highlightField(data[3].charAt(0) - '0');
    }
}
// Get logs of the game
function getLogs() {
    const packet = 'LOG';
    ws.send(packet);
}

// Generate game
generateGame();

/* WebSocket event listeners */

// Handle connection to server
ws.onopen = () => {
    console.log('Connected to server');
    ws.send('FETCH');
}
// Handle messages from server
ws.onmessage = (message) => {
    console.log(`Received server message: ${message.data}`);

    const [before, after] = message.data.split('?');
    const msgType = before;
    const data = after.split(';');

    // Handle message types
    switch (msgType) {
        case 'PLACE':
            if (data[0].toUpperCase() === 'TRUE') {
                updateCell(data[1], data[2]);
            } else {
                console.log('[Server] Invalid placement');
            }
            break;
        case 'WIN':
            if (data[0].toUpperCase() === 'TRUE') {
                console.log('[Server] Game over');
            } else if (data[0].toUpperCase() === 'TIE') {
                console.log('[Server] Tie');
            } else {
                highlightFields(data[1]);
            }
            break;
        case 'FETCH':
            syncBoardData(data);
            break;
        case 'LOG':
            console.log(data.toString());
            break;
        default:
            console.log(`Unknown server message type: ${msgType}`);
            break;
    }
}
// Handle disconnection from server
ws.onclose = () => {
    console.log('Disconnected from server');
}
// Handle errors
ws.onerror = (error) => {
    console.log('Error: ', error);
}
