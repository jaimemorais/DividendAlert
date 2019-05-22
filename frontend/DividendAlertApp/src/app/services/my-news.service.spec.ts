import { TestBed } from '@angular/core/testing';

import { MyNewsService } from './my-news.service';

describe('MyNewsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MyNewsService = TestBed.get(MyNewsService);
    expect(service).toBeTruthy();
  });
});
