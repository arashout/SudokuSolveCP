import React, { Component } from 'react';
import { Container } from 'reactstrap';

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
    return (
      <div>
        <Container className='d-flex justify-content-center'>
          {this.props.children}
        </Container>
      </div>
    );
  }
}
