const game = document.getElementsByClassName('game')[0];

// Create a 3x3 grid of fields
for (let row = 0; row < 3; row++) {
    const rowDiv = document.createElement('div');
    rowDiv.classList.add('row');
    for (let col = 0; col < 3; col++) {
        const cellDiv = document.createElement('div');
        cellDiv.classList.add('field');

        // Create a 3x3 grid of cells within each field
        for (let innerRow = 0; innerRow < 3; innerRow++) {
            const innerRowDiv = document.createElement('div');
            innerRowDiv.classList.add('inner-row');

            for (let innerCol = 0; innerCol < 3; innerCol++) {
                const innerCellDiv = document.createElement('div');
                innerCellDiv.classList.add('cell');
                innerRowDiv.appendChild(innerCellDiv);
            }
            cellDiv.appendChild(innerRowDiv);
        }

        rowDiv.appendChild(cellDiv);
    }

    game.appendChild(rowDiv);
}