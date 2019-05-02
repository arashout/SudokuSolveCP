import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'

export default class Grid extends PureComponent {
    static propTypes = {
        tiles: PropTypes.arrayOf(PropTypes.string)
    }

    renderTable = () => {
        const treArr = [];
        let tre;
        let tdArr;
        for (let i = 0; i < this.props.tiles.length; i++) {
            if (i % 9 === 0) {
                tdArr = [];
            }

            const tileValues = this.props.tiles[i];
            let td;
            if (tileValues.length === 1) {
                td = React.createElement('td', { key: 'd' + i }, tileValues);
            } else {
                td = React.createElement('td', { key: 'd'+i, className: 'text-info'}, tileValues)
            }
            tdArr.push(td);

            if (i % 9 === 8) {
                tre = React.createElement('tr', { key: 'r' + i }, tdArr);
                treArr.push(tre);
            }
        }
        const tbe = React.createElement('tbody', null, treArr);
        const te = React.createElement('table', { className: "table" }, tbe);
        return te;
    }

    render() {
        return (
            <div>
                {this.renderTable()}
            </div>
        )
    }
}
