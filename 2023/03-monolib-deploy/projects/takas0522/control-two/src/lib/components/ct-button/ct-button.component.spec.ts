import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CtButtonComponent } from './ct-button.component';

describe('CtButtonComponent', () => {
  let component: CtButtonComponent;
  let fixture: ComponentFixture<CtButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CtButtonComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CtButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
