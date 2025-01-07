package com.example.service;

import com.example.model.Message;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;

@Service
public class MessageService {
    private final List<Message> messages = new ArrayList<>();

    public void addMessage(Message message) {
        messages.add(message);
    }

    public Integer getMessagesCount() {
        return messages.size();
    }

    public boolean check(String id) {
        try {
            int index = Integer.parseInt(id);
            return index >= 0 && index < messages.size();
        } catch (NumberFormatException e) {
            return false;
        }
    }

    public List<Message> getAllMessages() {
        return messages.size() > 10 ? messages.subList(messages.size() - 10, messages.size()) : messages;
    }
}