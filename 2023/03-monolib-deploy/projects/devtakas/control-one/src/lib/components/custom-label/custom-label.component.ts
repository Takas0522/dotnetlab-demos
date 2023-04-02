import { Component, Input } from '@angular/core';

@Component({
  selector: 'lib-custom-label',
  templateUrl: './custom-label.component.html',
  styleUrls: ['./custom-label.component.css']
})
export class CustomLabelComponent {

  @Input()
  label = '';
}
