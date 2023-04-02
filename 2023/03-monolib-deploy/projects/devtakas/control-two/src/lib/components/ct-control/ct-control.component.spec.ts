import { CommonModule } from '@angular/common';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CtControlComponent } from './ct-control.component';

describe('CtControlComponent', () => {
  let component: CtControlComponent;
  let fixture: ComponentFixture<CtControlComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CtControlComponent ],
      imports: [ ReactiveFormsModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CtControlComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
