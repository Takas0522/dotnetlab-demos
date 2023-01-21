import { Component } from '@angular/core';

@Component({
  selector: 'cnt-button',
  template: `
    <button (click)="clickButton()"></button>
  `,
  styles: [
  ]
})
export class ControlsTwoComponent {

  clickButton() {
    alert('control2!')
  }
}
