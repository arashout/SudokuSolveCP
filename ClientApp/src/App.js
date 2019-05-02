import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import {SudokuSolver} from './components/SudokuSolver';

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route path='/' component={SudokuSolver} />
      </Layout>
    );
  }
}
