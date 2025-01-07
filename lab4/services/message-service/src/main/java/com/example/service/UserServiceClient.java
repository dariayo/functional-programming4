package com.example.service;

import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestTemplate;

@Component
@Slf4j
public class UserServiceClient {
    private final RestTemplate restTemplate;

    @Autowired
    public UserServiceClient(RestTemplate restTemplate) {
        this.restTemplate = restTemplate;
    }

    public boolean userExists(String username) {
        try {
            String url = "http://user-service:8084/users/" + username;
            return restTemplate.getForObject(url, String.class) != null;
        } catch (Exception e) {
            return false;
        }
    }
}
