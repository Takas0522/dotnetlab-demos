import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'lib-ct-control',
  templateUrl: './ct-control.component.html',
  styleUrls: ['./ct-control.component.css']
})
export class CtControlComponent {

  protected ctrl = new FormControl('')
}
