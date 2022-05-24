import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'front';

  constructor(
    private authService: AuthService
  ) {
    this.setUp();
  }
  ngOnInit(): void {
    this.setUp();
  }

  get userName$() {
    return this.authService.userName$;
  }

  async setUp() {
    await this.authService.clientSetup();
  }
}
