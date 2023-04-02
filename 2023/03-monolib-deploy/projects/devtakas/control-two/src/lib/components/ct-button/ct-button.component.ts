import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'lib-ct-button',
  templateUrl: './ct-button.component.html',
  styleUrls: ['./ct-button.component.css']
})
export class CtButtonComponent {

  @Input()
  buttonName = '';

  @Output()
  clickCtrl = new EventEmitter<void>()

  protected onClickButton() {
    this.clickCtrl.emit();
  }
}
