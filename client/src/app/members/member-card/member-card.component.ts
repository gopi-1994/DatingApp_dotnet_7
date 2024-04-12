import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { RouterModule} from '@angular/router';
import { MembersService } from 'src/app/_services/members.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit{
@Input() member: Member | undefined;


constructor(private memnberService: MembersService, private toastr: ToastrService) {}
ngOnInit():void {}

addLike(member: Member){
  this.memnberService.addLike(member.userName).subscribe({
    next: ()=> this.toastr.success('You have liked '+member.knownAs)
  })
}


}
