import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { AuthenticationService } from 'src/app/authentication/services/authentication.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss']
})
export class ConfirmEmailComponent implements OnInit, OnDestroy {
  isProgressSpinnerVisible = false;
  subscriptions: Subscription[] = [];

  constructor(
    private authenticationService: AuthenticationService,
    private messageService: MessageService,
    private route: ActivatedRoute,
    private router: Router) { }

  ngOnInit(): void {
    this.confirmEmail();
  }

  private confirmEmail(): void {
    const confirmEmail$ = this.authenticationService.confirmEmail(
      this.route.snapshot.queryParams['token'],
      this.route.snapshot.queryParams['email']
    ).pipe(
      tap(() => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Email confirmed'
        })
      })
    );

    this.subscriptions.push(this.doWithLoading(confirmEmail$).subscribe({
      complete: () => {
        this.router.navigate(['login']);
      }
    }))
  }

  private doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }
}
