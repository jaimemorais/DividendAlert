import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowDividendsComponent } from './show-dividends.component';

describe('ShowDividendsComponent', () => {
  let component: ShowDividendsComponent;
  let fixture: ComponentFixture<ShowDividendsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShowDividendsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowDividendsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
