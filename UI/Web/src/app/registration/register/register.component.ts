import { Component, OnInit } from '@angular/core';
import { UntypedFormGroup, UntypedFormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';
import { NavService } from 'src/app/_services/nav.service';

/**
 * This is exclusivly used to register the first user on the server and nothing else
 */
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  registerForm: UntypedFormGroup = new UntypedFormGroup({
    email: new UntypedFormControl('', [Validators.required, Validators.email]),
    username: new UntypedFormControl('', [Validators.required]),
    password: new UntypedFormControl('', [Validators.required, Validators.maxLength(32), Validators.minLength(6)]),
  });

  constructor(private route: ActivatedRoute, private router: Router, private accountService: AccountService, 
    private toastr: ToastrService, private memberService: MemberService) {
    this.memberService.adminExists().pipe(take(1)).subscribe(adminExists => {
      if (adminExists) {
        this.router.navigateByUrl('login');
        return;
      }
    });
  }

  ngOnInit(): void {
  }

  submit() {
    const model = this.registerForm.getRawValue();
    this.accountService.register(model).subscribe((user) => {
      this.toastr.success('Account registration complete');
      this.router.navigateByUrl('login');
    });
  }

}
