
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  users: any;
  registerMode = false;
  constructor(private http: HttpClient) {
  }
  ngOnInit(): void {
    // this.getUser(); 
  }
  registerToggle() {
    this.registerMode = !this.registerMode;
  }
  // commenting this due to baseusrl is set in src/environment
  // getUser() {
  //   this.http.get('https://localhost:5118/api/users').subscribe({
  //     next: response => this.users = response,
  //     error: error => console.log(error),
  //     complete: () => console.log('Requests has been completed')
  //   });
  // }
  cancelRegisterMode(event: boolean) {
    this.registerMode = event; 
  }
}
