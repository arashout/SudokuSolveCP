import React, { Component } from 'react';
import Grid from './Grid';
import PropTypes from 'prop-types'


export class SudokuSolver extends Component {
    static displayName = SudokuSolver.name;
    static defaultProps = {
        gridString: "200080300060070084030500209000105408000000000402706000301007040720040060004010003",
        isValid: true,
        tiles: null,
    }
    static propTypes = {
        gridString: PropTypes.string,
        isValid: PropTypes.bool,
        tiles: PropTypes.arrayOf(PropTypes.string),
    }

    constructor(props) {
        super(props);
        this.state = { gridString: this.props.gridString, loading: true, isValid: this.props.isValid };
    }

    constrain = () => {
        fetch(`api/constrain/${this.state.gridString}`)
            .then(response => response.json())
            .then(data => {
                this.setState({ tiles: data });
            });
    }

    solve = () => {
        fetch(`api/solve/${this.state.gridString}`)
            .then(response => response.json())
            .then(data => {
                this.setState({ tiles: data });
            });
    }

    reset = () => {
        this.setState({ tiles: null })
    }

    handleChange = (event) => {
        const value = event.target.value.trim();
        const isValid = value.length === 81 && Array.from(value).reduce((acc, s) => !isNaN(s) && acc, true)
        this.setState({ gridString: value, isValid: isValid });
    }

    render() {
        return (
            <div className='w-100 text-center'>
                <h1>Sudoku Solver <a href="https://github.com/arashout/SudokuSolveCP"><i class="fab fa-github"/></a></h1>
                <h4>Grid Input</h4>
                <div className='form-group'>
                    <textarea
                        className='form-control'
                        rows='2'
                        id="gridString"
                        maxLength={81}
                        value={this.state.gridString}
                        onChange={this.handleChange}
                    />
                </div>
                <h4>Grid Output</h4>
                <Grid tiles={this.state.tiles || Array.from(this.state.gridString)} />
                {
                    !this.state.isValid ?
                        <div className="d-flex justify-content-around">
                            <div className="alert alert-danger">Grid text is NOT 81 numbers</div>
                        </div>
                        :
                        <div className="d-flex justify-content-around">
                            <button className="btn btn-warning" onClick={this.reset}>Reset</button>
                            <button className="btn btn-secondary" onClick={this.constrain}>See Possible Values</button>
                            <button className="btn btn-primary" onClick={this.solve}>Solve</button>
                        </div>
                }

            </div>
        );
    }
}