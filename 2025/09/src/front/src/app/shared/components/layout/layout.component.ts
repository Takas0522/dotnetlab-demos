import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-layout',
  imports: [CommonModule, RouterOutlet],
  templateUrl: './layout.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LayoutComponent {
  private readonly router = inject(Router);

  ngOnInit() {
    // ルート変更を監視（必要に応じて）
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        // ページ変更時の処理があればここに追加
      });
  }

  navigateToDashboard() {
    this.router.navigate(['/dashboard']);
  }

  navigateToTodos() {
    this.router.navigate(['/todos']);
  }

  navigateToSharedTodos() {
    this.router.navigate(['/shared']);
  }
}
