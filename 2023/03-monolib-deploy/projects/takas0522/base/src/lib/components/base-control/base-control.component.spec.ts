import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { ControlOneModule } from '@takas0522/control-one';
import { ControlTwoModule } from '@takas0522/control-two';
import { UtilityModule } from '@takas0522/utility';

import { BaseControlComponent } from './base-control.component';

describe('BaseControlComponent', () => {
  let component: BaseControlComponent;
  let fixture: ComponentFixture<BaseControlComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BaseControlComponent ],
      imports: [ ReactiveFormsModule, ControlOneModule, ControlTwoModule, UtilityModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BaseControlComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
