import { HttpClient } from '@angular/common/http';
import { devOnlyGuardedExpression } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';


@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new BehaviorSubject<User | any>(null);
  currentUser$ = this.currentUserSource.asObservable();


  constructor(private http: HttpClient) { }
  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {// gets the users info & moves the login use data in applications local storage area
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }
  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if (user) {
         this.setCurrentUser(user);
        }
        return user;
      })
    )
  }
  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }
  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    
  }
}
