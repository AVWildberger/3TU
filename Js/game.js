const game = document.getElementsByClassName('game')[0];

let turn = 'X';

function generateGame() {
    // Create a 3x3 grid of fields
    for (let row = 0; row < 3; row++) {
        const rowDiv = document.createElement('div');
        rowDiv.classList.add('row');
        for (let col = 0; col < 3; col++) {
            const fieldDiv = document.createElement('div');
            fieldDiv.classList.add('field');

            // Create a 3x3 grid of cells within each field
            for (let innerRow = 0; innerRow < 3; innerRow++) {
                const innerRowDiv = document.createElement('div');
                innerRowDiv.classList.add('inner-row');

                for (let innerCol = 0; innerCol < 3; innerCol++) {
                    const cellDiv = document.createElement('div');
                    cellDiv.classList.add('cell');
                
                    const cellContent = document.createElement('p');

                    
                    const content = ' ';
                    cellContent.textContent = content;


                    // const contentOptions = [' ', 'X', 'O'];
                    // const content = contentOptions[Math.floor(Math.random() * contentOptions.length)];
                    // cellContent.textContent = content;
                
                    // if (content === 'X') {
                    //     cellDiv.style.backgroundColor = 'red';
                    // } else if (content === 'O') {
                    //     cellDiv.style.backgroundColor = 'lime';
                    // }

                    // Calculate the actual row and column for the cell
                    const actualRow = row * 3 + innerRow;
                    const actualCol = col * 3 + innerCol;

                    // Add click event listener to the cell
                    // cellDiv.addEventListener('click', () => {
                    //     logCellAsNotation(actualRow, actualCol, content);
                    // });
                    cellDiv.addEventListener('click', () => {
                        clickCell(actualRow, actualCol);
                    });
                
                    cellDiv.appendChild(cellContent);
                    innerRowDiv.appendChild(cellDiv);
                }
                fieldDiv.appendChild(innerRowDiv);
            }

            rowDiv.appendChild(fieldDiv);
        }

        game.appendChild(rowDiv);
    }
}

function getCellAsNotation(row, col, content) {
    const a = (row - (row % 3)) + ((col - (col % 3)) / 3) + 1;
    const b = (row % 3) * 3 + ((col % 3) + 1);

    return `${content}${a}${b}`;
}

function logCellAsNotation(row, col, content) {

    // console.log(`Row: ${row}, Col: ${col}`);
    console.log(getCellAsNotation(row, col, content));
}

function clickCell(row, col) {
    const cellDiv = document.getElementsByClassName('cell')[row * 9 + col];
    const cellContent = cellDiv.getElementsByTagName('p')[0];

    if (cellContent.textContent === ' ') {
        cellContent.textContent = turn;

        if (turn === 'X') {
            turn = 'O';
        } else {
            turn = 'X';
        }
    }
}

generateGame();