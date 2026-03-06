<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=!messagesPerField.existsError('username','password'); section>
    <#if section = "form">
    <div class="flex h-screen w-full">
        <!-- Left Side Image - Hidden on mobile -->
        <div class="w-full hidden md:inline-block">
            <img class="h-full w-full object-cover" src="https://raw.githubusercontent.com/prebuiltui/prebuiltui/main/assets/login/leftSideImage.png" alt="Login Image">
        </div>
        
        <!-- Right Side - Login Form -->
        <div class="w-full flex flex-col items-center justify-center px-4">
            <div class="md:w-96 w-80 flex flex-col items-center justify-center">
                <!-- Header -->
                <h2 class="text-4xl text-gray-900 font-medium">Sign in</h2>
                <p class="text-sm text-gray-500/90 mt-3">Welcome back! Please sign in to continue</p>

                <!-- Error/Success Messages -->
                <#if message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
                    <div class="w-full mt-6 rounded-lg p-4 ${(message.type = 'success')?then('bg-green-50 border border-green-200', 'bg-red-50 border border-red-200')}">
                        <p class="text-sm font-medium ${(message.type = 'success')?then('text-green-800', 'text-red-800')}">
                            ${kcSanitize(message.summary)?no_esc}
                        </p>
                    </div>
                </#if>

                <!-- Social Login (Google) - Only if enabled -->
                <#if realm.password && social.providers??>
                    <#list social.providers as p>
                        <#if p.alias == "google">
                            <a href="${p.loginUrl}" class="w-full mt-8 bg-gray-500/10 flex items-center justify-center h-12 rounded-full hover:bg-gray-500/20 transition-colors">
                                <img src="https://raw.githubusercontent.com/prebuiltui/prebuiltui/main/assets/login/googleLogo.svg" alt="Google Logo">
                            </a>
                        </#if>
                    </#list>

                    <!-- Divider -->
                    <div class="flex items-center gap-4 w-full my-5">
                        <div class="w-full h-px bg-gray-300/90"></div>
                        <p class="w-full text-nowrap text-sm text-gray-500/90">or sign in with email</p>
                        <div class="w-full h-px bg-gray-300/90"></div>
                    </div>
                <#else>
                    <div class="mt-8"></div>
                </#if>

                <!-- Login Form -->
                <form class="w-full" action="${url.loginAction}" method="post">
                    <input type="hidden" name="credentialId" <#if auth.selectedCredential?has_content>value="${auth.selectedCredential}"</#if>/>
                    
                    <!-- Email/Username Field -->
                    <div class="flex items-center w-full bg-transparent border border-gray-300/60 h-12 rounded-full overflow-hidden pl-6 gap-2">
                        <svg width="16" height="11" viewBox="0 0 16 11" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path fill-rule="evenodd" clip-rule="evenodd" d="M0 .55.571 0H15.43l.57.55v9.9l-.571.55H.57L0 10.45zm1.143 1.138V9.9h13.714V1.69l-6.503 4.8h-.697zM13.749 1.1H2.25L8 5.356z" fill="#6B7280"/>
                        </svg>
                        <input 
                            id="username"
                            name="username" 
                            type="text" 
                            value="${(login.username!'')}"
                            placeholder="Email id" 
                            autocomplete="username"
                            class="bg-transparent text-gray-500/80 placeholder-gray-500/80 outline-none text-sm w-full h-full pr-6" 
                            autofocus
                            required
                        >                 
                    </div>

                    <!-- Password Field -->
                    <div class="flex items-center mt-6 w-full bg-transparent border border-gray-300/60 h-12 rounded-full overflow-hidden pl-6 gap-2">
                        <svg width="13" height="17" viewBox="0 0 13 17" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M13 8.5c0-.938-.729-1.7-1.625-1.7h-.812V4.25C10.563 1.907 8.74 0 6.5 0S2.438 1.907 2.438 4.25V6.8h-.813C.729 6.8 0 7.562 0 8.5v6.8c0 .938.729 1.7 1.625 1.7h9.75c.896 0 1.625-.762 1.625-1.7zM4.063 4.25c0-1.406 1.093-2.55 2.437-2.55s2.438 1.144 2.438 2.55V6.8H4.061z" fill="#6B7280"/>
                        </svg>
                        <input 
                            id="password"
                            name="password" 
                            type="password" 
                            placeholder="Password" 
                            autocomplete="current-password"
                            class="bg-transparent text-gray-500/80 placeholder-gray-500/80 outline-none text-sm w-full h-full pr-6" 
                            required
                        >
                    </div>

                    <!-- Remember Me & Forgot Password -->
                    <div class="w-full flex items-center justify-between mt-8 text-gray-500/80">
                        <#if realm.rememberMe && !usernameHidden??>
                            <div class="flex items-center gap-2">
                                <input 
                                    class="h-5 w-5 rounded border-gray-300 text-indigo-500 focus:ring-indigo-500" 
                                    type="checkbox" 
                                    id="rememberMe"
                                    name="rememberMe"
                                    <#if login.rememberMe??>checked</#if>
                                >
                                <label class="text-sm" for="rememberMe">Remember me</label>
                            </div>
                        <#else>
                            <div></div>
                        </#if>

                        <#if realm.resetPasswordAllowed>
                            <a class="text-sm underline hover:text-gray-700" href="${url.loginResetCredentialsUrl}">Forgot password?</a>
                        </#if>
                    </div>

                    <!-- Login Button -->
                    <button 
                        type="submit" 
                        name="login"
                        class="mt-8 w-full h-11 rounded-full text-white bg-indigo-500 hover:opacity-90 transition-opacity font-medium"
                    >
                        Login
                    </button>
                </form>
            </div>
        </div>
    </div>
    </#if>
</@layout.registrationLayout>