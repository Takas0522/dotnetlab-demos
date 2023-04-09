import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { UtilityModule } from '@takas0522/utility';

import { CustomOneComponent } from './custom-one.component';

describe('CustomOneComponent', () => {
  let component: CustomOneComponent;
  let fixture: ComponentFixture<CustomOneComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CustomOneComponent ],
      imports: [
        ReactiveFormsModule,
        UtilityModule
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomOneComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
